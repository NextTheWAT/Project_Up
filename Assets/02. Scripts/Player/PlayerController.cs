using System;
using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerController : MonoBehaviour
{
    [Header("Movement")]
    public float walkSpeed = 4f;
    public float runSpeed = 7f;
    public float moveSpeed;
    public Vector2 curMovementInput;
    public float jumpPower = 5f;
    public LayerMask groundLayerMask;

    [Header("Look (Orbit Camera)")]
    public Transform cameraContainer;   // 플레이어의 자식 피벗(그 아래 실제 Camera가 있어도 됨)
    public float minXLook = -50f;
    public float maxXLook = 70f;
    public float lookSensitivity = 0.1f;

    [Header("Third-Person Camera")]
    public float cameraHeight = 1.6f;   // 카메라 기준 높이
    public float targetDistance = 3.5f; // 기본 거리
    public float minDistance = 1.0f;
    public float maxDistance = 6.0f;
    public float zoomSpeed = 2.0f;
    public float followSmooth = 10f;    // 카메라 위치 보간
    public LayerMask cameraCollisionMask; // 벽 등 충돌 레이어

    [Header("ReStart")]
    public Transform reStartPoint; // 재시작 지점 (플레이어가 죽었을 때 리스폰 위치)

    [Header("Stamina")]
    public bool isRunning = false; // 현재 달리기 중인지
    public float staminaDrainRate = 10f; // 달릴 때 소모되는 스태미너

    private float camCurXRot;           // pitch 누적
    private Vector2 mouseDelta;
    [HideInInspector] public bool canLook = true;

    public Action inventory;
    private Rigidbody rigidbody;

    void Awake() => rigidbody = GetComponent<Rigidbody>();

    void Start()
    {
        Cursor.lockState = CursorLockMode.Locked;
        moveSpeed = walkSpeed;
        // 초기 카메라 피벗 높이 세팅
        if (cameraContainer != null)
            cameraContainer.localPosition = new Vector3(0f, cameraHeight, 0f);
    }

    void Update()
    {
        HandleZoom();
        RunUseStamina();
    }

    void FixedUpdate() => Move();

    void LateUpdate()
    {
        if (canLook) CameraLook();
        UpdateThirdPersonCamera();
    }

    // ===== Input =====
    public void OnLookInput(InputAction.CallbackContext context)
    {
        mouseDelta = context.ReadValue<Vector2>();
    }

    public void OnMoveInput(InputAction.CallbackContext context)
    {
        if (context.phase == InputActionPhase.Performed)
            curMovementInput = context.ReadValue<Vector2>();
        else if (context.phase == InputActionPhase.Canceled)
            curMovementInput = Vector2.zero;
    }

    public void OnRun(InputAction.CallbackContext context)
    {
        if (context.phase == InputActionPhase.Performed)
        {
            isRunning = true;
            moveSpeed = runSpeed;
        }
        else if (context.phase == InputActionPhase.Canceled)
        {
            isRunning = false;
            moveSpeed = walkSpeed;
        }
    }


    public void OnJumpInput(InputAction.CallbackContext context)
    {
        if (context.phase == InputActionPhase.Started && IsGrounded())
            rigidbody.AddForce(Vector3.up * jumpPower, ForceMode.Impulse);
    }

    public void OnInventory(InputAction.CallbackContext context)
    {
        if (context.phase == InputActionPhase.Started)
        {
            inventory?.Invoke();
            ToggleCursor();
        }
    }

    public void OnReStart(InputAction.CallbackContext context)
    {
        if(context.phase == InputActionPhase.Started)
        {
            ReStart();
        }
    }

    private void ReStart()
    {
        CharacterManager.Instance.Player.transform.position = reStartPoint.position;
    }

    private void RunUseStamina()
    {
        if (isRunning && curMovementInput.magnitude > 0.1f) // 이동 중 + 달리기 상태
        {
            CharacterManager.Instance.Player.condition.UseStamina(Time.deltaTime * staminaDrainRate);
        }
    }

    // ===== Movement (카메라 기준) =====
    private void Move()
    {
        // 카메라의 평면 기준으로 이동 방향을 만든다
        Vector3 camForward = cameraContainer != null ? cameraContainer.forward : transform.forward;
        Vector3 camRight = cameraContainer != null ? cameraContainer.right : transform.right;

        camForward.y = 0f; camRight.y = 0f;
        camForward.Normalize(); camRight.Normalize();

        Vector3 dir = camForward * curMovementInput.y + camRight * curMovementInput.x;
        dir *= moveSpeed;
        dir.y = rigidbody.velocity.y;

        rigidbody.velocity = dir;
    }

    // ===== Camera Look (Yaw = 플레이어, Pitch = 카메라 피벗) =====
    void CameraLook()
    {
        camCurXRot += mouseDelta.y * lookSensitivity;
        camCurXRot = Mathf.Clamp(camCurXRot, minXLook, maxXLook);

        // Pitch
        if (cameraContainer != null)
        {
            var localEuler = cameraContainer.localEulerAngles;
            cameraContainer.localEulerAngles = new Vector3(-camCurXRot, localEuler.y, 0);
        }

        // Yaw (플레이어 본체 회전)
        transform.eulerAngles += new Vector3(0, mouseDelta.x * lookSensitivity, 0);
    }

    // ===== Third-Person Follow + 충돌 보정 =====
    void UpdateThirdPersonCamera()
    {
        if (cameraContainer == null) return;

        // 피벗을 플레이어 위치(+높이)로 스무스하게 보간
        Vector3 desiredPivot = transform.position + Vector3.up * cameraHeight;
        cameraContainer.position = Vector3.Lerp(cameraContainer.position, desiredPivot, Time.deltaTime * followSmooth);

        // 원하는 카메라 위치(피벗 뒤쪽으로 targetDistance만큼)
        Vector3 desiredCamPos = cameraContainer.position - cameraContainer.forward * targetDistance;

        // 충돌 체크: 피벗 → 원하는 위치로 Ray/SphereCast 해서 벽에 닿으면 더 앞으로 당김
        float adjustedDistance = targetDistance;
        if (Physics.SphereCast(cameraContainer.position, 0.15f, -cameraContainer.forward, out RaycastHit hit, targetDistance, cameraCollisionMask))
        {
            adjustedDistance = Mathf.Clamp(hit.distance - 0.1f, minDistance, targetDistance);
        }

        Vector3 finalCamPos = cameraContainer.position - cameraContainer.forward * adjustedDistance;

        // 실제 카메라 Transform이 cameraContainer 자신인 경우: position만 옮김
        // 만약 Camera가 cameraContainer의 자식이라면, 자식 카메라를 -distance로 배치하는 방식으로 바꿔도 됨.
        Camera mainCam = Camera.main;
        if (mainCam != null)
        {
            mainCam.transform.position = Vector3.Lerp(mainCam.transform.position, finalCamPos, Time.deltaTime * followSmooth);
            mainCam.transform.rotation = Quaternion.Slerp(mainCam.transform.rotation, Quaternion.LookRotation(cameraContainer.forward, Vector3.up), Time.deltaTime * followSmooth);
        }
    }

    // ===== Zoom (마우스 휠) =====
    void HandleZoom()
    {
        if (Mouse.current == null) return;
        float scroll = Mouse.current.scroll.ReadValue().y; // 위로 굴리면 +, 아래 - (기기마다 반대일 수 있음)
        if (Mathf.Abs(scroll) > 0.01f)
        {
            targetDistance -= (scroll / 120f) * zoomSpeed; // 일반 휠 한 칸이 보통 120
            targetDistance = Mathf.Clamp(targetDistance, minDistance, maxDistance);
        }
    }

    // ===== Ground Check =====
    public bool IsGrounded()
    {
        Ray[] rays = new Ray[4]
        {
            new Ray(transform.position + (transform.forward * 0.2f) + (transform.up * 0.01f), Vector3.down),
            new Ray(transform.position + (-transform.forward * 0.2f) + (transform.up * 0.01f), Vector3.down),
            new Ray(transform.position + (transform.right * 0.2f) + (transform.up * 0.01f), Vector3.down),
            new Ray(transform.position + (-transform.right * 0.2f) +(transform.up * 0.01f), Vector3.down)
        };

        for (int i = 0; i < rays.Length; i++)
            if (Physics.Raycast(rays[i], 0.1f, groundLayerMask))
                return true;

        return false;
    }

    // ===== Cursor Toggle =====
    public void ToggleCursor(bool toggle)
    {
        Cursor.lockState = toggle ? CursorLockMode.None : CursorLockMode.Locked;
        canLook = !toggle;
    }

    void ToggleCursor()
    {
        bool toggle = Cursor.lockState == CursorLockMode.Locked;
        Cursor.lockState = toggle ? CursorLockMode.None : CursorLockMode.Locked;
        canLook = !toggle;
    }
}
