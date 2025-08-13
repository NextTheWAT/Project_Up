using UnityEngine;

[RequireComponent(typeof(Collider))]
public class JumpPad : MonoBehaviour
{
    [Header("점프 세기")]
    public float jumpForce = 10f;
    public ForceMode forceMode = ForceMode.Impulse;

    [Header("설정")]
    public string targetTag = "Player"; // 플레이어 태그
    public bool resetVerticalVelocity = true; // 기존 y속도 초기화 여부

    [Header("애니메이션")]
    public Animator anim;

    private void Reset()
    {
        // 자동으로 트리거 모드로 설정
        GetComponent<Collider>().isTrigger = true;
    }
    private void Start()
    {
        anim = GetComponent<Animator>();
    }

    private void OnTriggerEnter(Collider other)
    {
        // 플레이어 태그만 처리
        if (!string.IsNullOrEmpty(targetTag) && !other.CompareTag(targetTag)) return;

        Rigidbody rb = other.attachedRigidbody;
        if (rb != null)
        {
            // 기존 y속도 제거(낙하 중일 때 더 깔끔하게 튕김)
            if (resetVerticalVelocity)
            {
                Vector3 v = rb.velocity;
                v.y = 0f;
                rb.velocity = v;
            }
            anim.SetTrigger("JumpTrigger");

            // 위로 힘 가하기
            rb.AddForce(Vector3.up * jumpForce, forceMode);
        }
    }
}
