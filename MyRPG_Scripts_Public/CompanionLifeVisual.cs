using UnityEngine;

public class CompanionLifeVisual : MonoBehaviour
{
    [Header("ステータス")]
    public CharacterStats stats;

    [Header("見た目")]
    public Animator animator;
    public SpriteRenderer spriteRenderer;

    public Sprite normalSprite;   // 生きてるときの見た目
    public Sprite coffinSprite;   // 棺桶の見た目

    [Header("スケール設定")]
    public Vector3 normalScale = Vector3.one;
    public Vector3 coffinScale = new Vector3(0.7f, 0.7f, 1f);


    [Header("会話系スクリプト（任意）")]
    public MonoBehaviour[] talkScripts;

    // 内部保存用
    Vector3 initialLocalPos;

    void Awake()
    {
        if (stats == null)
            stats = GetComponent<CharacterStats>();

        if (spriteRenderer == null)
            spriteRenderer = GetComponentInChildren<SpriteRenderer>();

        if (animator == null)
            animator = GetComponentInChildren<Animator>();

        // 今の見た目を生きてる見た目として保存
        if (spriteRenderer != null && normalSprite == null)
        {
            if (normalSprite == null)
                normalSprite = spriteRenderer.sprite;

            // スプライトの元のローカル座標を保存
            initialLocalPos = spriteRenderer.transform.localPosition;

        }

    }

    // 仲間が死亡したときに呼ぶ
    public void SwitchToCoffin()
    {
        Debug.Log("[CompanionLifeVisual] 棺桶見た目に切り替え");

        // アニメ止める
        if (animator != null)
            animator.enabled = false;

        // スプライトONにして棺桶に変更＋スケール変更
        if (spriteRenderer != null)
        {
            spriteRenderer.enabled = true;

            if (coffinSprite != null)
                spriteRenderer.sprite = coffinSprite;

            // 棺桶用のサイズに変更
            transform.localScale = coffinScale;
        }


        // 会話スクリプトを止める
        if (talkScripts != null)
        {
            foreach (var s in talkScripts)
            {
                if (s != null) s.enabled = false;
            }
        }
    }

    // 回復で生き返ったときに呼ぶ
    public void ReviveVisual()
    {
        Debug.Log("[CompanionLifeVisual] 見た目・アニメを復活前に戻す");

        if (animator != null)
            animator.enabled = true;

        if (spriteRenderer != null)
        {
            spriteRenderer.enabled = true;

            if (normalSprite != null)
                spriteRenderer.sprite = normalSprite;

            // 生きてるときのサイズに戻す
            transform.localScale = normalScale;
        }


        if (talkScripts != null)
        {
            foreach (var s in talkScripts)
            {
                if (s != null) s.enabled = true;
            }
        }

    }
}