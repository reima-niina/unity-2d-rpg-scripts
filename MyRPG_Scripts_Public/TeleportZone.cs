using System.Collections;
using UnityEngine;
using UnityEngine.VFX;

public class TeleportZone : MonoBehaviour
{
    [Header("ワープ先の位置（ここにプレイヤーを飛ばす）")]
    public Transform destination;

    [Header("一緒にワープさせたい仲間（任意）")]
    public Transform companion;

    [Header("ワープさせる対象のタグ")]
    public string playerTag = "Player";

    [Header("フェード時間（片道）")]
    public float fadeDuration = 0.5f;

    [Header("真っ黒で止めておく時間（秒）")]
    public float blackHoldDuration = 0.5f;

    [Header("カメラ（省略したら Camera.main を使う）")]
    public Transform cameraTransform;

    // ★ どのエリア用BGMを鳴らしたいか（Inspector で選ぶ）
    public enum AreaType
    {
        None,
        Village,
        Field,
        Forest,
        Castle
    }

    [Header("ワープ後のエリア種別（BGM切替用）")]
    public AreaType targetArea = AreaType.None;

    [Header("ワープ時のSE（任意）")]
    public AudioClip warpSE;

    // 全テレポート共通で今ワープ中かどうかを見る
    private static bool isTeleporting = false;

    private void OnTriggerEnter2D(Collider2D other)
    {
        // 指定タグじゃなければ無視
        if (!other.CompareTag(playerTag)) return;

        // すでにどこかでワープ中なら無視（連続ワープ防止）
        if (isTeleporting) return;

        if (destination == null)
        {
            Debug.LogWarning($"[TeleportZone] destination が設定されていません: {name}");
            return;
        }

        StartCoroutine(TeleportRoutine(other));
    }

    private IEnumerator TeleportRoutine(Collider2D other)
    {
        isTeleporting = true;

        // プレイヤーTransform取得
        Transform playerTr = other.attachedRigidbody != null
            ? other.attachedRigidbody.transform
            : other.transform;

        // 仲間とのオフセットを保存
        Vector3 companionOffset = Vector3.zero;
        bool hasCompanion = (companion != null);

        if (hasCompanion)
        {
            companionOffset = companion.position - playerTr.position;
        }

        // 使うカメラを決定
        Transform camTr = cameraTransform;
        if (camTr == null && Camera.main != null)
        {
            camTr = Camera.main.transform;
        }

        // ★ ワープSEを鳴らす（あれば）
        if (warpSE != null && SEManager.Instance != null)
        {
            SEManager.Instance.PlaySE(warpSE);
        }


        // 1) フェードアウト
        if (ScreenFader.Instance != null)
        {
            yield return ScreenFader.Instance.FadeOut(fadeDuration);

        }

        // 2) 真っ黒になった状態でワープさせる
        playerTr.position = destination.position;

        if (hasCompanion)
        {
            companion.position = destination.position + companionOffset;
        }

        // 3) カメラも一気にワープ先へスナップさせる
        if (camTr != null)
        {
            Vector3 cp = camTr.position;
            camTr.position = new Vector3(
                playerTr.position.x,
                playerTr.position.y,
                cp.z                  // Z だけそのまま（カメラの奥行き）
            );
        }

        // ★ 4) ここで BGM 変更（真っ黒の間に切り替える）
        if (BGMManager.Instance != null)
        {
            switch (targetArea)
            {
                case AreaType.Village:
                    BGMManager.Instance.PlayVillageBGM();
                    break;
                case AreaType.Field:
                    BGMManager.Instance.PlayFieldBGM();
                    break;
                case AreaType.Forest:
                    BGMManager.Instance.PlayForestBGM();
                    break;
                case AreaType.Castle:
                    BGMManager.Instance.PlayCastleBGM();
                    break;
                case AreaType.None:
                default:
                    // 何もしない
                    break;
            }
        }


        // 4) カメラが落ち着いているあいだ、真っ黒のまま少し待つ
        if (blackHoldDuration > 0f)
        {
            yield return new WaitForSeconds(blackHoldDuration);
        }


        // 5) フェードイン
        if (ScreenFader.Instance != null)
        {
            yield return ScreenFader.Instance.FadeIn(fadeDuration);
        }

        isTeleporting = false;
    }
}