// CameraFollow.cs みたいな名前で MainCamera に付ける
using UnityEngine;

public class CameraFollow : MonoBehaviour
{
    [Header("追いかける対象（プレイヤー）")]
    public Transform target;

    [Header("追尾の設定")]
    public float smooth = 5f;

    Vector3 offset;

    void Start()
    {
        if (target != null)
        {
            offset = transform.position - target.position;
        }
    }

    void LateUpdate()
    {
        
        if (target == null) return;

        Vector3 targetPos = target.position + offset;
        transform.position = Vector3.Lerp(
            transform.position,
            targetPos,
            smooth * Time.deltaTime
        );
    }
}