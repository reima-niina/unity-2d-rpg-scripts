using UnityEngine;

public class EnemyAI : MonoBehaviour
{
    [Header("この敵自身のステータス")]
    public CharacterStats enemyStats;   // 自分（敵）

    [Header("攻撃対象（プレイヤー & 仲間）のステータス")]
    public CharacterStats playerStats;      // プレイヤー
    public CharacterStats companionStats;   // 仲間

    [Header("バトル画面用アニメ")]
    public Animator battleAnimator;

    [Header("行動の基本パラメータ")]
    public int normalAttackPower = 5;


    private void Awake()
    {
        // 自分の CharacterStats を自動取得
        if (enemyStats == null)
        {
            enemyStats = GetComponent<CharacterStats>();
            if (enemyStats == null)
            {
                Debug.LogWarning("[EnemyAI] CharacterStats がこの敵に付いていません");
            }
        }
    }

    private void EnsureTargets()
    {
        if (enemyStats == null)
        {
            enemyStats = GetComponent<CharacterStats>();
        }

        if (BattleManager.Instance != null)
        {
            // プレイヤー & 仲間のステータスを BattleManager からもらう
            if (playerStats == null)
            {
                playerStats = BattleManager.Instance.playerStats;
            }
            if (companionStats == null)
            {
                companionStats = BattleManager.Instance.companionStats;
            }
        }
    }
}