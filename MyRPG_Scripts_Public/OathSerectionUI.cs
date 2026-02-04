using UnityEngine;

public class OathSelectionUI : MonoBehaviour
{
    // このパネル（自分自身）を入れておく
    public GameObject panel;

    // どのスキルのために開いているのか
    private OathSkillExample pendingSkill;

    private void Start()
    {
        // 最初は閉じておく
        if (panel != null)
        {
            panel.SetActive(false);
        }
    }

    /// <summary>
    /// スキル側から呼ばれる
    /// </summary>
    public void Open(OathSkillExample skill)
    {
        pendingSkill = skill;

        if (panel != null)
        {
            panel.SetActive(true);
        }

        // 必要なら一時停止
        Time.timeScale = 0f;
    }

    // ここから下はボタンごとに呼ぶメソッド 

    // 「最大HP -20」の誓約ボタン
    public void ChooseMaxHpDown()
    {
        ConfirmSelection(OathType.MaxHPMinus20);
    }

    // 「攻撃力 -5」の誓約ボタン
    public void ChooseAttackDown()
    {
        ConfirmSelection(OathType.AttackMinus5);
    }

    public void ChooseDefenseDown()
    {
        ConfirmSelection(OathType.DefenseMinus5);
    }

    /// <summary>
    /// 実際に誓約を確定する処理
    /// </summary>
    private void ConfirmSelection(OathType oath)
    {
        // 1. UIを閉じる
        if (panel != null)
        {
            panel.SetActive(false);
        }
        Time.timeScale = 1f;

        // 2. 対応するスキルに「この誓約で確定した」と知らせる
        if (pendingSkill != null)
        {
            pendingSkill.OnOathConfirmed(oath);
            pendingSkill = null;
        }
    }

    // キャンセルボタン
    public void Cancel()
    {
        if (panel != null)
        {
            panel.SetActive(false);
        }
        Time.timeScale = 1f;
        pendingSkill = null;
    }
}