using UnityEngine;
using TMPro;

public class LastBossDialogue : MonoBehaviour
{
    [Header("UI への参照")]
    public GameObject dialogPanel;      // 会話ウィンドウ
    public TMP_Text nameText;           // 名前表示
    public TMP_Text bodyText;           // セリフ本文

    [Header("ラスボス設定")]
    public string bossName = "ベルフェゴール";    // 名前
    [TextArea(2, 5)]
    public string[] lines;              // 開始前の会話

    [Header("バトル用")]
    public CharacterStats bossStats;    // このボスのステータス
    public KeyCode talkKey = KeyCode.Z;

    // 内部状態
    bool playerInRange = false;
    bool isOpen = false;
    int currentIndex = 0;
    bool battleStarted = false;         // 二重起動防止

    void Start()
    {
        if (dialogPanel != null)
        {
            dialogPanel.SetActive(false);
        }
    }

    void Update()
    {
        if (!playerInRange) return;
        if (battleStarted) return;  // もうバトル始まってたら何もしない

        if (Input.GetKeyDown(talkKey))
        {
            if (!isOpen)
            {
                OpenDialogue();
            }
            else
            {
                ShowNextLine();
            }
        }
    }

    void OpenDialogue()
    {
        if (dialogPanel == null || nameText == null || bodyText == null)
        {
            Debug.LogWarning("[LastBossDialogue] UI 参照が足りません");
            return;
        }

        isOpen = true;
        currentIndex = 0;

        dialogPanel.SetActive(true);
        nameText.text = bossName;

        if (lines != null && lines.Length > 0)
        {
            bodyText.text = lines[currentIndex];
        }
        else
        {
            bodyText.text = "……（何かを見据えている）";
        }
    }

    void ShowNextLine()
    {
        if (lines == null || lines.Length == 0)
        {
            StartBossBattle();
            return;
        }

        currentIndex++;

        if (currentIndex < lines.Length)
        {
            bodyText.text = lines[currentIndex];
        }
        else
        {
            // 会話が終わったタイミングでバトル開始
            StartBossBattle();
        }
    }

    void StartBossBattle()
    {
        if (battleStarted) return;
        battleStarted = true;

        // 会話ウィンドウを閉じる
        if (dialogPanel != null)
        {
            dialogPanel.SetActive(false);
        }
        isOpen = false;

        if (bossStats == null)
        {
            Debug.LogError("[LastBossDialogue] bossStats が設定されていません");
            return;
        }

        if (BattleManager.Instance == null)
        {
            Debug.LogError("[LastBossDialogue] BattleManager.Instance が見つかりません");
            return;
        }

        // ここでバトル開始
        BattleManager.Instance.StartBattle(bossStats, gameObject);
    }

    // 会話可能範囲の判定（村人と同じ）
    void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            playerInRange = true;
            Debug.Log("[LastBossDialogue] プレイヤーが話しかけ可能範囲に入った");
        }
    }

    void OnTriggerExit2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            playerInRange = false;
            if (isOpen && dialogPanel != null)
            {
                dialogPanel.SetActive(false);
            }
            isOpen = false;
        }
    }
}