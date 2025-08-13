using UnityEngine;
using DG.Tweening;

public class Move_SportCar : MonoBehaviour
{
    public enum MoveMode { BackAndForth, Circle }
    public enum CirclePlane { XZ, XY, YZ }

    [Header("공통")]
    [SerializeField] private MoveMode mode = MoveMode.BackAndForth;
    [SerializeField] private bool playOnStart = true;
    [SerializeField] private Ease ease = Ease.Linear;

    [Header("앞뒤 왕복 (BackAndForth)")]
    [SerializeField] private bool useLocal = true;
    [SerializeField] private float distance = 5f;
    [SerializeField] private float duration = 2f;
    [SerializeField] private Vector3 direction = Vector3.forward;
    [SerializeField] private LoopType loopType = LoopType.Yoyo;

    [Header("원형(Circle)")]
    [SerializeField] private CirclePlane plane = CirclePlane.XZ;
    [SerializeField] private Transform center;
    [SerializeField] private float radius = 3f;
    [SerializeField] private float cycleSeconds = 3f;
    [SerializeField] private bool clockwise = true;

    [Header("휠 회전")]
    [SerializeField] private Transform[] wheels; // 회전시킬 휠
    [SerializeField] private float wheelRotateSpeed = 360f; // 초당 회전 각도

    private Tweener _moveTween;
    private Tween _circleTween;
    private Vector3 _startPos;
    private Vector3 _lastPos;

    private void Awake()
    {
        _startPos = useLocal ? transform.localPosition : transform.position;
        _lastPos = _startPos;
    }

    private void Start()
    {
        if (playOnStart) Play();
    }

    public void Play()
    {
        KillTweens();

        if (mode == MoveMode.BackAndForth)
        {
            Vector3 dir = direction.sqrMagnitude < 0.0001f ? Vector3.forward : direction.normalized;

            if (useLocal)
            {
                Vector3 from = _startPos;
                Vector3 to = _startPos + dir * distance;

                _moveTween = transform.DOLocalMove(to, duration)
                    .SetEase(ease)
                    .SetLoops(-1, loopType)
                    .From(from)
                    .OnUpdate(UpdateWheelRotation_BackAndForth);
            }
            else
            {
                Vector3 from = _startPos;
                Vector3 to = _startPos + dir * distance;

                _moveTween = transform.DOMove(to, duration)
                    .SetEase(ease)
                    .SetLoops(-1, loopType)
                    .From(from)
                    .OnUpdate(UpdateWheelRotation_BackAndForth);
            }
        }
        else // Circle
        {
            Vector3 c = (center ? (useLocal ? center.localPosition : center.position)
                                : _startPos);

            float fromAngle = 0f;
            float toAngle = clockwise ? -360f : 360f;

            _circleTween = DOVirtual.Float(fromAngle, toAngle, cycleSeconds, angle =>
            {
                float rad = angle * Mathf.Deg2Rad;
                Vector3 offset = Vector3.zero;

                switch (plane)
                {
                    case CirclePlane.XZ: offset = new Vector3(Mathf.Cos(rad), 0f, Mathf.Sin(rad)) * radius; break;
                    case CirclePlane.XY: offset = new Vector3(Mathf.Cos(rad), Mathf.Sin(rad), 0f) * radius; break;
                    case CirclePlane.YZ: offset = new Vector3(0f, Mathf.Cos(rad), Mathf.Sin(rad)) * radius; break;
                }

                if (useLocal) transform.localPosition = c + offset;
                else transform.position = c + offset;

                // 원형 모드: 무조건 앞으로 간다고 가정하고 X+ 회전
                RotateWheels(+1);
            })
            .SetEase(Ease.Linear)
            .SetLoops(-1, LoopType.Restart);
        }
    }

    private void UpdateWheelRotation_BackAndForth()
    {
        Vector3 currentPos = useLocal ? transform.localPosition : transform.position;
        float dir = Mathf.Sign(Vector3.Dot(currentPos - _lastPos, direction.normalized));

        if (dir != 0) // 방향이 정해졌으면
        {
            RotateWheels(dir);
        }

        _lastPos = currentPos;
    }

    private void RotateWheels(float directionSign)
    {
        if (wheels == null || wheels.Length == 0) return;

        float deltaAngle = wheelRotateSpeed * directionSign * Time.deltaTime;
        foreach (var wheel in wheels)
        {
            if (wheel != null)
                wheel.Rotate(Vector3.right, deltaAngle, Space.Self);
        }
    }

    public void Stop()
    {
        KillTweens();
    }

    private void OnDisable()
    {
        KillTweens();
    }

    private void KillTweens()
    {
        _moveTween?.Kill();
        _circleTween?.Kill();
        _moveTween = null;
        _circleTween = null;
    }
}
