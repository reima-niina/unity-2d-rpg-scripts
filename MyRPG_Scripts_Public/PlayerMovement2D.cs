using Unity.VisualScripting;
using UnityEngine;

[RequireComponent(typeof(Rigidbody2D))]
[RequireComponent(typeof(Animator))]
[RequireComponent(typeof(SpriteRenderer))]
public class PlayerMovement2D : MonoBehaviour
{
    public float moveSpeed = 3f;

    private Rigidbody2D rb;
    private Vector2 input;
    public bool canMove = true;
    public Animator animator;
    public SpriteRenderer spriteRenderer;

    private void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
        animator = GetComponent<Animator>();
        spriteRenderer = GetComponent<SpriteRenderer>();
    }

    private void Start()
    {
        // 強制で true に初期化（Inspector の古い値をリセット）
        canMove = true;
    }

    private void Update()
    {
        // カットシーン・バトルなど、動けないとき
        if (!canMove)
        {
            input = Vector2.zero;

            if (rb != null)
            {
                rb.linearVelocity = Vector2.zero;
            }

            // ここではアニメにあまり触らない（Speedだけ0にする程度）
            if (animator != null)
            {
                animator.SetFloat("Speed", 0f);
            }
            return;  // ここで終了
        }

        // ここから下は「動けるときだけ」実行される

        float h = Input.GetAxisRaw("Horizontal");
        float v = Input.GetAxisRaw("Vertical");
        input = new Vector2(h, v).normalized;

        bool isMoving = input.sqrMagnitude > 0.01f;

        if (animator != null)
        {
            animator.SetBool("IsMoving", isMoving);

            if (isMoving)
            {
                animator.SetFloat("MoveX", input.x);
                animator.SetFloat("MoveY", input.y);
                animator.SetFloat("Speed", input.sqrMagnitude);
            }
            else
            {
                animator.SetFloat("Speed", 0f);
            }
        }

        // 左右反転（横方向のとき）
        if (input.x > 0.1f)
        {
            spriteRenderer.flipX = false;  // 右向き
        }
        else if (input.x < -0.1f)
        {
            spriteRenderer.flipX = true;   // 左向き
        }
    }

    private void FixedUpdate()
    {
        if (rb != null)
        {
            rb.linearVelocity = input * moveSpeed;
        }
    }

    // 外から止めたいとき用
    public void StopMovement()
    {
        canMove = false;
        input = Vector2.zero;

        if (rb != null)
        {
            rb.linearVelocity = Vector2.zero;
        }

        if (animator != null)
        {
            animator.SetFloat("Speed", 0f);
            animator.SetBool("IsMoving", false);
        }
    }

    // 外から再開したいとき用
    public void ResumeMovement()
    {
        canMove = true;
    }
}