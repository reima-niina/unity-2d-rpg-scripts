
using UnityEngine;

public class OathSkillExample : MonoBehaviour
{
    [Header("プレイヤーのステータス")]
    public CharacterStats playerStats;     // Inspector で Player をドラッグ

    [Header("誓約UI")]
    public OathSelectionUI oathSelectionUI;

    // 今バトル中の敵はいつもここから取る
    private CharacterStats CurrentEnemy
    {
        get
        {
            return BattleManager.Instance != null
                ? BattleManager.Instance.enemyStats
                : null;
        }
    }

    /// <summary>
    /// 「たたかう」ボタンから呼ぶ
    /// </summary>
    public void DoNormalAttack()
    {
        
     
    
        // ここから先はバトルのメイン処理を BattleManager に任せる
        if (BattleManager.Instance != null)
        {
            BattleManager.Instance.OnAttackButton();
        }
        else
        {
            Debug.LogWarning("[OathSkillExample] BattleManager.Instance が見つかりません");
        }
    
}

    /// <summary>
    /// 禁術ボタンから呼ぶ
    /// </summary>
    public void TryUseOathSkill()
    {
        if (oathSelectionUI != null)
        {
            oathSelectionUI.Open(this);
        }
        else
        {
            Debug.LogWarning("OathSelectionUI が設定されていません");
        }
    }

    /// <summary>
    /// 誓約UIで選択が確定したときに呼ばれる
    /// </summary>
    public void OnOathConfirmed(OathType oath)
    {
        var enemy = CurrentEnemy;

        if (playerStats == null || enemy == null)
        {
            Debug.LogWarning("OnOathConfirmed: playerStats または enemy が設定されていません");
            return;
        }

        // 誓約を適用（以後も継続）
        if (OathManager.Instance != null && OathManager.Instance.targetCharacter != null)
        {
            // OathManager 経由でプレイヤーに誓約をセット
            OathManager.Instance.SetOath(oath);
        }
        else
        {
            // 念のため　OathManager が未設定でも動くように
            playerStats.ApplyOath(oath);
        }

        // 誓約適用後の攻撃力で禁術ダメージを計算
        int atk = playerStats.attack; 
        int def = enemy.defense;
        int damage;

        switch (oath)
        {
            case OathType.MaxHPMinus20:
                // HPを削る代わりに、ちょっと強い禁術
                damage = Mathf.Max(1, Mathf.RoundToInt(atk * 3f - def));
                break;

            case OathType.AttackMinus5:
                // 攻撃を恒久的に下げる代わりに、さらに強い禁術
                damage = Mathf.Max(1, Mathf.RoundToInt(atk * 4f + 5 - def));
                break;

            case OathType.DefenseMinus5:
                // 守りを捨てて敵DEFを無視して殴る
                damage = Mathf.Max(1, Mathf.RoundToInt(atk * 2f)); 
                break;

            default:
                // 念のため
                damage = Mathf.Max(1, atk * 3 - def);
                break;
        }


        enemy.TakeRawDamage(damage);

        string msg =
            $"禁術発動！ [{oath}] {enemy.characterName} に {damage} ダメージ（HP {enemy.currentHP}/{enemy.maxHP}）";
        Debug.Log(msg);
        BattleLogUI.Instance?.AddLog(msg);

        if (enemy.IsDead())
        {
            BattleLogUI.Instance?.AddLog($"{enemy.characterName} を たおした！");

            // ここでキルカウントを増やす
            if (playerStats != null)
            {
                playerStats.RegisterKill();
            }

            // 仲間にもキルを入れる
            if (BattleManager.Instance != null && BattleManager.Instance.companionStats != null)
            {
                BattleManager.Instance.companionStats.RegisterKill();
            }


            BattleManager.Instance?.EndBattle();
            return;
        }

        // 敵がまだ生きているなら「仲間→敵ターン」を BattleManager に任せる
        if (BattleManager.Instance != null)
        {
            BattleManager.Instance.StartPostOathTurn();
        }
    }
}