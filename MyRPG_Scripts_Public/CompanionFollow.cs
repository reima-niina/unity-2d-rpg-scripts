using UnityEngine;

[RequireComponent(typeof(Animator))]
[RequireComponent(typeof(SpriteRenderer))]
[RequireComponent(typeof(Rigidbody2D))]
public class CompanionFollow : MonoBehaviour
{
    [Header("追いかける対象（プレイヤー）")]
    public Transform target;

    [Header("追従パラメータ")]
    public float followDistance = 1.5f;   // 理想の距離
    public float moveSpeed = 3f;          // 仲間の移動速度
    public float stopEpsilon = 0.2f;      // この幅のぶんは動かない帯にする

    [Header("カットシーン制御")]
    public bool isInCutscene = false;

    Rigidbody2D rb;
    Animator animator;
    SpriteRenderer spriteRenderer;

    Vector2 lastMoveDir = Vector2.down;

    void Awake()
    {
        rb = GetComponent<Rigidbody2D>();

        animator = GetComponent<Animator>();

        spriteRenderer = GetComponent<SpriteRenderer>();
        if (spriteRenderer == null)
        {
            spriteRenderer = GetComponentInChildren<SpriteRenderer>();
        }

        if (animator == null)
        {
            Debug.LogError("[CompanionFollow] Animator が見つかりません", this);
        }
        else if (animator.runtimeAnimatorController == null)
        {
            Debug.LogError("[CompanionFollow] Animator に Controller が設定されていません", this);
        }
    }

    void Update()
    {
        if (target == null) return;
        if (animator == null || spriteRenderer == null) return;
        if (animator.runtimeAnimatorController == null) return;

        // カットシーン中は「追いかけ処理を全部スキップ」
        if (isInCutscene)
        {
            // 物理的な移動は止める
            rb.linearVelocity = Vector2.zero;
            return; // アニメは ScenarioManager 側から制御する
        }


        Vector2 currentPos = transform.position;
        Vector2 targetPos = target.position;
        Vector2 toTarget = targetPos - currentPos;

        float dist = toTarget.magnitude;
        bool isMoving = false;
        Vector2 moveDir = lastMoveDir;

        // デッドゾーン付きの判定
        //  followDistance より十分に遠いときだけ動く
        if (dist > followDistance + stopEpsilon)
        {
            moveDir = toTarget.normalized;

            Vector2 newPos = Vector2.MoveTowards(
                currentPos,
                targetPos,
                moveSpeed * Time.deltaTime
            );

            transform.position = newPos;
            isMoving = true;
            lastMoveDir = moveDir;
        }
        else
        {
            // この帯の中では動かない扱いにする
            isMoving = false;
        }

        UpdateAnimation(isMoving, moveDir);
    }

    void UpdateAnimation(bool isMoving, Vector2 dir)
    {
        if (animator == null || spriteRenderer == null) return;
        if (animator.runtimeAnimatorController == null) return;

        // 動いてないときアニメの再生速度を0にして、そのコマで停止
        if (!isMoving || dir.sqrMagnitude < 0.0001f)
        {
            return;
        }

        // ここから下はこれまでどおり（WalkUp / WalkDown / WalkSide の分岐）
        if (Mathf.Abs(dir.y) > Mathf.Abs(dir.x))
        {
            // 上下メイン
            if (dir.y > 0)
            {
                animator.Play("Companion_WalkUp");
            }
            else
            {
                animator.Play("Companion_WalkDown");
            }
            spriteRenderer.flipX = false;
        }
        else
        {
            // 左右メイン
            animator.Play("Companion_WalkSide");

            if (dir.x < 0)
            {
                spriteRenderer.flipX = true;   // 左向き
            }
            else if (dir.x > 0)
            {
                spriteRenderer.flipX = false;  // 右向き
            }
        }
    }

    public void SetCutsceneMode(bool inCutscene)
    {
        isInCutscene = inCutscene;

        if (inCutscene && rb != null)
        {
            // カットシーンに入った瞬間に一度止めておく
            rb.linearVelocity = Vector2.zero;
        }

    }
}
