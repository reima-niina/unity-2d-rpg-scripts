using UnityEngine;

public class EncounterZone : MonoBehaviour
{
    public CharacterStats enemyStats;   // この敵のステータス

    bool hasTriggered = false;

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (hasTriggered) return;
        if (!other.CompareTag("Player")) return;

        hasTriggered = true;

        if (BattleManager.Instance != null)
        {
            Debug.Log($"[EncounterZone] {enemyStats.characterName} とバトル開始");
            // enemyStats（この敵のステータス）と、自分自身(gameObject)を渡す
            BattleManager.Instance.StartBattle(enemyStats, this.gameObject);
        }
    }
}
