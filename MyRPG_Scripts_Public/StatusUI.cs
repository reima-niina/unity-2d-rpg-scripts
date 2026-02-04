using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class StatusUI : MonoBehaviour
{
    [Header("プレイヤー")]
    public CharacterStats target;   // プレイヤーのStatsをドラッグ

    public TMP_Text nameText;
    public TMP_Text levelText;
    public TMP_Text hpText;
    public TMP_Text attackText;
    public TMP_Text defenseText;
    public TMP_Text nextLevelText;
    public TMP_Text oathText;

    [Header("仲間")]
    public CharacterStats companionTarget;          // 仲間のStatsをドラッグ

    public TMP_Text companionNameText;
    public TMP_Text companionLevelText;
    public TMP_Text companionHpText;
    public TMP_Text companionAttackText;
    public TMP_Text companiondefenseText;
    public TMP_Text companionNextLevelText;

    private void OnEnable()
    {
        Refresh();
    }

    private void Update()
    {
        // 開いている間は常に更新
        Refresh();
    }

    public void Refresh()
    {
        // プレイヤー側
        if (target != null)
        {
            if (nameText != null)
                nameText.text = target.characterName;

            if (levelText != null)
                levelText.text = $"Lv {target.level}";

            if (hpText != null)
                hpText.text = $"HP {target.currentHP} / {target.maxHP}";

            if (attackText != null)
                attackText.text = $"ATK {target.attack}";

            if (defenseText != null)
                defenseText.text = $"DEF {target.defense}";

            if (nextLevelText != null)
            {
                int remaining = target.killsToNextLevel - target.currentKillCount;
                if (remaining < 0) remaining = 0;
                nextLevelText.text = $"次のレベルまで あと {remaining} 体";
            }

            // 誓約ペナルティ表示（HP -40 / ATK -10 みたいに）
            if (oathText != null)
            {
                int hpPenalty = target.oathMaxHpPenalty;    // 例えば -40
                int atkPenalty = target.oathAttackPenalty;  // 例えば -10
                int defPenalty = target.oathDefensePenalty;

                if (hpPenalty == 0 && atkPenalty == 0 && defPenalty == 0)
                {
                    oathText.text = "誓約ペナルティ：なし";
                }
                else
                {
                    string hpPart = hpPenalty != 0 ? $"HP -{Mathf.Abs(hpPenalty)}" : "";
                    string atkPart = atkPenalty != 0 ? $"ATK -{Mathf.Abs(atkPenalty)}" : "";
                    string defPart = defPenalty != 0 ? $"DEF -{Mathf.Abs(defPenalty)}" : "";

                    string combined = "";

                    if (hpPart != "")
                    {
                        combined += hpPart;
                    }
                    if (atkPart != "")
                    {
                        if (combined != "") combined += " / ";
                        combined += atkPart;
                    }
                    if (defPart != "")
                    {
                        if (combined != "") combined += " / ";
                        combined += defPart;
                    }


                        oathText.text = $"誓約ペナルティ：{combined}";
                }
            }
        }

        // 仲間側
        if (companionTarget != null)
        {
            if (companionNameText != null)
                companionNameText.text = companionTarget.characterName;

            if (companionLevelText != null)
                companionLevelText.text = $"Lv {companionTarget.level}";

            if (companionHpText != null)
                companionHpText.text =
                    $"HP {companionTarget.currentHP} / {companionTarget.maxHP}";

            if (companionAttackText != null)
                companionAttackText.text = $"ATK {companionTarget.attack}";

            if (companiondefenseText != null)
                companiondefenseText.text = $"DEF {companionTarget.defense}";

            if (companionNextLevelText != null)
            {
                int remaining = companionTarget.killsToNextLevel - companionTarget.currentKillCount;
                if (remaining < 0) remaining = 0;
                companionNextLevelText.text = $"次のレベルまで あと {remaining} 体";
            }
        }

    }

    // ボタンから呼ぶ用（ステータス画面のON/OFF）
    public void Toggle()
    {
        gameObject.SetActive(!gameObject.activeSelf);
    }
}
