using UnityEngine;

[RequireComponent(typeof(Animator))]
public class EnemyAnimRandomizer : MonoBehaviour
{
    [Header("再生させるステート名（Idle用）")]
    public string idleStateName = "EnemyIdle";  // Animator内のステート名

    [Header("アニメ再生スピード")]
    public float minSpeed = 0.8f;
    public float maxSpeed = 1.2f;

    private Animator animator;

    void Start()
    {
        animator = GetComponent<Animator>();
        if (animator == null) return;

        // 0.0〜1.0 のランダムな位置から再生開始
        float randomOffset = Random.value;

        // レイヤー0、normalizedTime = randomOffset でステートを再生
        animator.Play(idleStateName, 0, randomOffset);

        // すぐに反映させるために一回 Update(0)
        animator.Update(0f);

        // 再生スピードも個体差をつける
        animator.speed = Random.Range(minSpeed, maxSpeed);
    }
}