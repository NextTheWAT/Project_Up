using UnityEngine;

[RequireComponent(typeof(Animator))]
public class PlayerAnimation : MonoBehaviour
{
    private Animator animator;
    private PlayerController playerController;
    private Rigidbody rb;

    private void Awake()
    {
        animator = GetComponent<Animator>();
        playerController = GetComponent<PlayerController>();
        rb = GetComponent<Rigidbody>();
    }

    private void Update()
    {
        UpdateAnimator();
    }

    private void UpdateAnimator()
    {
        Vector3 flatVelocity = new Vector3(rb.velocity.x, 0f, rb.velocity.z);
        float actualSpeed = flatVelocity.magnitude;

        bool isWalking = actualSpeed > 0.1f && actualSpeed < 5.0f; // 걷는 속도 범위
        bool isRunning = actualSpeed >= 5.0f; // 달리는 속도 범위

        animator.SetBool("Walk", isWalking);

        animator.SetBool("Run", isRunning);

        // 땅에 있는지
        bool isGrounded = IsGrounded();
        animator.SetBool("Grounded", isGrounded);

        // 점프 입력 판단 (IsGrounded가 false이고, y속도가 양수면 점프 중)
        bool isJumping = !isGrounded && rb.velocity.y > 0.1f;
        animator.SetBool("Jump", isJumping);

        // 낙하 상태 판단
        bool isFalling = !isGrounded && rb.velocity.y < -0.1f;
        animator.SetBool("FreeFall", isFalling);
    }

    private bool IsGrounded()
    {
        return playerController.IsGrounded();
    }

}
