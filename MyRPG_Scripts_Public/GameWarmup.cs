using System.Collections;
using UnityEngine;

public class GameWarmup : MonoBehaviour
{
    IEnumerator Start()
    {
        // 1フレーム待ってからやると、他のシングルトンがAwakeし終わってる
        yield return null;

        // BGM のウォームアップ
        if (BGMManager.Instance != null)
        {
            var src = BGMManager.Instance.bgmSource;
            if (src != null && BGMManager.Instance.fieldBGM != null)
            {
                float bakVol = src.volume;
                src.volume = 0f; // 聞こえないように
                src.clip = BGMManager.Instance.fieldBGM;
                src.Play();
                yield return new WaitForSeconds(0.1f);
                src.Stop();
                src.volume = bakVol;
            }
        }

        // バトルUI のウォームアップ
        if (BattleManager.Instance != null && BattleManager.Instance.battleUI != null)
        {
            var ui = BattleManager.Instance.battleUI;

            // 一瞬だけONにしてすぐOFF → TMPやレイアウトを先に作らせる
            ui.SetActive(true);
            yield return null;  // 1フレームだけ描画させる
            ui.SetActive(false);
        }

        // フェード（ScreenFader）のウォームアップ
        if (ScreenFader.Instance != null)
        {
            // ほぼ一瞬のフェードで、内部の Canvas / マテリアルだけ準備させる
            yield return ScreenFader.Instance.FadeOut(0.01f);
            yield return ScreenFader.Instance.FadeIn(0.01f);
        }

        Debug.Log("[GameWarmup] ウォームアップ完了");
    }
}