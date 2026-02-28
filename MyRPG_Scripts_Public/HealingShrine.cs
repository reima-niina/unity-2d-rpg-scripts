using System.Collections;
using UnityEngine;

public class HealingShrine : MonoBehaviour
{
    [Header("プレイヤー")]
    public PlayerMovement2D playerMove;   // プレイヤーの移動スクリプト
    public CharacterStats playerStats;    // プレイヤーのステータス（HP をいじる）

    [Header("仲間")] 
    public CharacterStats companionStats; // 仲間のステータス
    public CompanionLifeVisual companionVisual;

    [Header("会話（なくてもOK）")]
    public StoryCutscene talkCutscene;    // 話しかけたときの会話用 StoryCutscene

    [Header("操作キー")]
    public KeyCode talkKey = KeyCode.Z;

    bool playerInRange = false;   // プレイヤーが範囲内にいるか
    bool isRunning = false;       // 今このイベントが進行中か

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            playerInRange = true;
            Debug.Log("[HealingShrine] プレイヤーが範囲内に入った");
        }
    }

    private void OnTriggerExit2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            playerInRange = false;
            Debug.Log("[HealingShrine] プレイヤーが範囲外に出た");
        }
    }

    private void Update()
    {
        if (!playerInRange) return;
        if (isRunning) return;

        if (Input.GetKeyDown(talkKey))
        {
            StartCoroutine(HealingFlow());
        }
    }

    IEnumerator HealingFlow()
    {
        isRunning = true;

        // プレイヤーの操作ストップ
        if (playerMove != null)
        {
            playerMove.canMove = false;
        }

        // ① 会話パート
        if (talkCutscene != null)
        {
            Debug.Log("[HealingShrine] 会話開始");
            yield return StartCoroutine(talkCutscene.PlayOpening());
        }

        // ② 暗転 → HP回復 → 明転
        if (ScreenFader.Instance != null)
        {
            // 暗くする
            yield return ScreenFader.Instance.FadeOut(0.8f);
        }

        // ここで HP を全回復（誓約ペナルティには触らない）
        if (playerStats != null)
        {
            playerStats.ResetHP();
            Debug.Log($"[HealingShrine] {playerStats.characterName} のHPを全回復: {playerStats.currentHP}/{playerStats.maxHP}");
        }

        // 仲間も同じように全回復
        if (companionStats != null)
        {
            bool wasDead = companionStats.currentHP <= 0;

            companionStats.ResetHP();
            Debug.Log($"[HealingShrine] {companionStats.characterName} のHPを全回復: {companionStats.currentHP}/{companionStats.maxHP}");

            // 死んでいた仲間なら見た目も復活させる
            if (wasDead && companionVisual != null)
            {
                companionVisual.ReviveVisual();
            }

        }

        // ちょっと待ってから戻す（演出用）
        yield return new WaitForSeconds(0.5f);

        if (ScreenFader.Instance != null)
        {
            // 元に戻す
            yield return ScreenFader.Instance.FadeIn(0.8f);
        }

        // プレイヤー操作再開
        if (playerMove != null)
        {
            playerMove.canMove = true;
        }

        isRunning = false;
    }
}