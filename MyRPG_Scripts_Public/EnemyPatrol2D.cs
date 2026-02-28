using UnityEngine;

public class EnemyPatrol2D : MonoBehaviour
{
    [Header("移動スピード")]
    public float moveSpeed = 1f;

    [Header("スタート位置からの最大移動範囲（X,Y方向）")]
    public Vector2 moveRange = new Vector2(2f, 2f);

    private Vector3 startPos;
    private Vector3 targetPos;

    void Start()
    {
        // 最初にいた場所を基準にする
        startPos = transform.position;

        // 最初の目的地を決める
        PickNewTarget();
    }

    void Update()
    {
        // ターゲットに向かって移動
        transform.position = Vector3.MoveTowards(
            transform.position,
            targetPos,
            moveSpeed * Time.deltaTime
        );

        // 目的地にだいたい着いたら、次の目的地を決める
        if (Vector3.Distance(transform.position, targetPos) < 0.05f)
        {
            PickNewTarget();
        }
    }

    private void PickNewTarget()
    {
        float offsetX = Random.Range(-moveRange.x, moveRange.x);
        float offsetY = Random.Range(-moveRange.y, moveRange.y);

        targetPos = startPos + new Vector3(offsetX, offsetY, 0f);
    }
}