using System.Collections;
using UnityEngine;

public class BossBattleController : MonoBehaviour
{
    [Header("ボスのステータス")]
    public CharacterStats bossStats;          // このボス用の CharacterStats

    [Header("プレイヤー")]
    public PlayerMovement2D playerMove;       // プレイヤーの移動スクリプト
    public Transform playerTransform;         // プレイヤーの Transform
    public float talkRange = 1.5f;            // どのくらいの距離で話しかけられるか

    [Header("会話 & エンディング（StoryCutscene を割り当てる）")]
    public StoryCutscene preBattleCutscene;   // 戦闘前の会話
    public StoryCutscene postBattleCutscene;  // 戦闘後の会話
    public StoryCutscene endingCutscene;      // 真っ黒画面で流すエンディング

    [Header("見た目")]
    public GameObject bossVisualRoot;         // ボスの見た目オブジェクト

    [Header("操作キー")]
    public KeyCode talkKey = KeyCode.Z;

    // 内部状態
    private bool isRunning = false;           // このボスイベントが進行中か
    private bool isCleared = false;           // すでに倒しているか

    private void Reset()
    {
        if (bossStats == null)
            bossStats = GetComponent<CharacterStats>();

        if (bossVisualRoot == null)
            bossVisualRoot = this.gameObject;
    }

    private void Update()
    {
        if (isCleared) return;   // もう倒している
        if (isRunning) return;   // イベント進行中
        if (playerTransform == null) return; // プレイヤー未設定

        // 距離で話しかけ可能か判定
        float dist = Vector2.Distance(
            new Vector2(transform.position.x, transform.position.y),
            new Vector2(playerTransform.position.x, playerTransform.position.y)
        );

        if (dist > talkRange) return; // 遠すぎる

        // Z が押されたら会話開始
        if (Input.GetKeyDown(talkKey))
        {
            Debug.Log("[BossBC] Z入力検知 → BossFlow開始");
            StartCoroutine(BossFlow());
        }
    }

    private IEnumerator BossFlow()
    {
        isRunning = true;
        Debug.Log("[BossBC] BossFlow START");

        // プレイヤー操作停止
        if (playerMove != null)
        {
            playerMove.canMove = false;
        }

        // 1) 戦闘前会話
        if (preBattleCutscene != null)
        {
            Debug.Log("[BossBC] preBattleCutscene 再生");
            yield return StartCoroutine(preBattleCutscene.PlayOpening());
        }

        // 2) バトル開始
        if (BattleManager.Instance != null && bossStats != null)
        {
            Debug.Log("[BossBC] StartBattle 呼び出し");
            BattleManager.Instance.StartBattle(bossStats, bossVisualRoot);
        }
        else
        {
            Debug.LogError("[BossBC] BattleManager または bossStats が設定されていません");
            if (playerMove != null) playerMove.canMove = true;
            isRunning = false;
            yield break;
        }

        // 3) バトル終了待ち
        while (BattleManager.Instance != null && BattleManager.Instance.InBattle)
        {
            yield return null;
        }

        Debug.Log("[BossBC] バトル終了検知");

        // バトルが終わったら、勝敗に関係なくここに返ってくる
        // ここでは「もう二度と話しかけて発動しない」だけしておく
        isCleared = bossStats != null && bossStats.IsDead();

        // 負けていた場合など、必要ならプレイヤー操作を戻す
        if (!isCleared && playerMove != null)
        {
            playerMove.canMove = true;
        }

        isRunning = false;
    }

    // ギズモで範囲を見えるように
    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, talkRange);
    }
}