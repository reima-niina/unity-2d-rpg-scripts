using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class HPBarUI : MonoBehaviour
{
    [Header("このHPバーが参照するキャラ")]
    public CharacterStats target;

    [Header("UI部品")]
    public Slider slider;      // HPバー本体
    public TMP_Text hpText;    // 30 / 50 の文字
    public Image fillImage;    // バーの色を変える対象

    private void Awake()
    {
        // 同じオブジェクトに付いてる Slider を自動取得
        if (slider == null)
        {
            slider = GetComponent<Slider>();
        }

        // fillImage が未設定なら Slider から自動取得
        if (fillImage == null && slider != null && slider.fillRect != null)
        {
            fillImage = slider.fillRect.GetComponent<Image>();
        }
    }

    private void Start()
    {
        if (target == null)
        {
            Debug.LogWarning($"{name} (HPBarUI): target が設定されていません");
            return;
        }

        if (slider == null)
        {
            Debug.LogWarning($"{name} (HPBarUI): slider が見つかりません");
            return;
        }

        slider.minValue = 0;
        slider.maxValue = target.maxHP;
        slider.value = target.currentHP;

        slider.wholeNumbers = true;

        UpdateText();
        UpdateColor();
    }

    private void Update()
    {
        if (target == null || slider == null) return;

        slider.maxValue = target.maxHP;
        slider.value = Mathf.Clamp(target.currentHP, 0, target.maxHP);

        UpdateText();
        UpdateColor();
    }

    private void UpdateText()
    {
        if (hpText == null || target == null) return;
        hpText.text = $"{target.currentHP} / {target.maxHP}";
    }

    private void UpdateColor()
    {
        if (fillImage == null || target == null) return;

        float ratio = (float)target.currentHP / Mathf.Max(1, target.maxHP);

        // ここで割合ごとに色を変える
        if (ratio > 0.5f)
        {
            // 50%以上 → 緑
            fillImage.color = Color.green;
        }
        else if (ratio > 0.2f)
        {
            // 20〜50% → 黄色
            fillImage.color = Color.yellow;
        }
        else
        {
            // 20%以下 → 赤
            fillImage.color = Color.red;
        }
    }

    public void SetTarget(CharacterStats newTarget)
    {
        target = newTarget;

        if (target != null && slider != null)
        {
            slider.minValue = 0;
            slider.maxValue = target.maxHP;
            slider.value = target.currentHP;

            UpdateText();
            UpdateColor();
        }
    }

}