using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class NPCDialogue : MonoBehaviour
{
    [Header("UI への参照")]
    public GameObject dialogPanel;  // 会話ウィンドウ用パネル
    public TMP_Text nameText;           // 名前表示
    public TMP_Text bodyText;           // セリフ本文

    [Header("NPC 設定")]
    public string npcName = "村人";    // Inspector から変えられる
    [TextArea(2, 5)]
    public string[] lines;             // セリフの配列（順番に再生）

    [Header("操作キー")]
    public KeyCode talkKey = KeyCode.Z;

    [Header("メインシナリオ用設定")]
    public bool isMainQuestVillager = false;
    private bool hasReported = false;

    // 内部状態
    int currentIndex = 0;
    bool playerInRange = false;   // プレイヤーが近いか
    bool isOpen = false;          // 会話ウィンドウが開いてるか

    void Start()
    {
        if (dialogPanel != null)
        {
            dialogPanel.SetActive(false);  // 最初は閉じておく
        }
    }

    void Update()
    {
        if (!playerInRange) return;

        // Zキー押されたときの挙動
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
        if (dialogPanel == null || bodyText == null || nameText == null)
        {
            Debug.LogWarning("NPCDialogue: UI の参照が足りません");
            return;
        }

        isOpen = true;
        currentIndex = 0;

        dialogPanel.SetActive(true);
        nameText.text = npcName;

        if (lines != null && lines.Length > 0)
        {
            bodyText.text = lines[currentIndex];
        }
        else
        {
            bodyText.text = "……（何も話すことがないようだ）";
        }
    }

    void ShowNextLine()
    {
        if (lines == null || lines.Length == 0)
        {
            CloseDialogue();
            return;
        }

        currentIndex++;

        // まだセリフが残っている
        if (currentIndex < lines.Length)
        {
            bodyText.text = lines[currentIndex];
        }
        else
        {
            // 最後まで行ったら閉じる
            CloseDialogue();
        }
    }

    void CloseDialogue()
    {
        isOpen = false;
        currentIndex = 0;

        if (dialogPanel != null)
        {
            dialogPanel.SetActive(false);
        }


        if (isMainQuestVillager && !hasReported)
        {
            hasReported = true;

            if (ScenarioManager.Instance != null)
            {
                ScenarioManager.Instance.NotifyTalkedVillager();
            }
        }
    }

    // プレイヤーが近づいたら会話可能にする
    void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            playerInRange = true;
            // ここで「Zで話しかけよう」みたいな表示出すかも
            Debug.Log("話しかけられる距離に入りました");
        }
    }

    // 離れたら会話できなくする
    void OnTriggerExit2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            playerInRange = false;
            if (isOpen)
            {
                CloseDialogue();
            }
        }
    }
}