using System.Collections;
using UnityEngine;
using UnityEngine.UI;

public class ScreenFader : MonoBehaviour
{
    public static ScreenFader Instance;

    [Header("フェードに使う CanvasGroup")]
    public CanvasGroup canvasGroup;

    [Header("デフォルトのフェード時間（秒）")]
    public float defaultDuration = 0.5f;

    void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            //DontDestroyOnLoad(gameObject); // シーンまたぐならここ有効
        }
        else
        {
            Destroy(gameObject);
        }
    }

    void Start()
    {
        if (canvasGroup == null)
        {
            canvasGroup = GetComponent<CanvasGroup>();
        }

        if (canvasGroup != null)
        {
            // 最初は透明＆クリックを邪魔しない
            canvasGroup.alpha = 0f;
            canvasGroup.blocksRaycasts = false;
        }
    }

    public IEnumerator FadeOut(float duration)
    {
        if (canvasGroup == null) yield break;

        // フェード中はクリックをブロック
        canvasGroup.blocksRaycasts = true;

        float t = 0f;
        while (t < duration)
        {
            t += Time.deltaTime;
            float a = Mathf.Clamp01(t / duration);
            canvasGroup.alpha = a;
            yield return null;
        }
        canvasGroup.alpha = 1f;
    }

    public IEnumerator FadeIn(float duration)
    {
        if (canvasGroup == null) yield break;

        float t = 0f;
        while (t < duration)
        {
            t += Time.deltaTime;
            float a = 1f - Mathf.Clamp01(t / duration);
            canvasGroup.alpha = a;
            yield return null;
        }
        canvasGroup.alpha = 0f;

        // 完全に明るくなったらクリック解放
        canvasGroup.blocksRaycasts = false;
    }
}