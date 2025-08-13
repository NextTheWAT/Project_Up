using UnityEngine;

[RequireComponent(typeof(Collider))]
public class JumpPad : MonoBehaviour
{
    [Header("���� ����")]
    public float jumpForce = 10f;
    public ForceMode forceMode = ForceMode.Impulse;

    [Header("����")]
    public string targetTag = "Player"; // �÷��̾� �±�
    public bool resetVerticalVelocity = true; // ���� y�ӵ� �ʱ�ȭ ����

    [Header("�ִϸ��̼�")]
    public Animator anim;

    private void Reset()
    {
        // �ڵ����� Ʈ���� ���� ����
        GetComponent<Collider>().isTrigger = true;
    }
    private void Start()
    {
        anim = GetComponent<Animator>();
    }

    private void OnTriggerEnter(Collider other)
    {
        // �÷��̾� �±׸� ó��
        if (!string.IsNullOrEmpty(targetTag) && !other.CompareTag(targetTag)) return;

        Rigidbody rb = other.attachedRigidbody;
        if (rb != null)
        {
            // ���� y�ӵ� ����(���� ���� �� �� ����ϰ� ƨ��)
            if (resetVerticalVelocity)
            {
                Vector3 v = rb.velocity;
                v.y = 0f;
                rb.velocity = v;
            }
            anim.SetTrigger("JumpTrigger");

            // ���� �� ���ϱ�
            rb.AddForce(Vector3.up * jumpForce, forceMode);
        }
    }
}
