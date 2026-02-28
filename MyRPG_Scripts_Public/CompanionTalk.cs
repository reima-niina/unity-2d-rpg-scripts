using UnityEngine;
using TMPro;

public class CompanionTalk : MonoBehaviour
{
    [Header("UI 参照")]
    public GameObject talkPanel;           // CompanionTalkPanel を入れる
    public TextMeshProUGUI nameText;
    public TextMeshProUGUI bodyText;   // CompanionTalkText を入れる

    [Header("仲間の名前")]
    public string speakerName = "セルリアン";

    [Header("セリフ候補（ランダムで1つ選ぶ）")]
    [TextArea(2, 5)]
    public string[] randomLines;

    [Header("操作設定")]
    public KeyCode talkKey = KeyCode.Z;    // 会話ボタン（Zキー）

    bool isPlayerInRange = false;          // プレイヤーが話しかけ可能範囲にいるか
    bool isTalking = false;                // 今、会話ウィンドウが開いているか
    int lastIndex = -1;                    // 直前に出したセリフ（同じの連続を避ける用）

    void Start()
    {
        if (talkPanel != null)
        {
            talkPanel.SetActive(false);    // 最初は非表示
        }
    }

    void Update()
    {
        if (!isPlayerInRange) return;

        // Zキーが押されたら
        if (Input.GetKeyDown(talkKey))
        {
            if (!isTalking)
            {
                StartTalk();
            }
            else
            {
                EndTalk();
            }
        }
    }

    void StartTalk()
    {
        if (randomLines == null || randomLines.Length == 0)
        {
            Debug.LogWarning("CompanionTalk: randomLines が設定されていません");
            return;
        }

        // ランダムで 1 つ選ぶ（前回と同じになりにくくする）
        int index = Random.Range(0, randomLines.Length);
        if (randomLines.Length > 1)
        {
            // 連続で同じセリフを避ける
            int safety = 0;
            while (index == lastIndex && safety < 10)
            {
                index = Random.Range(0, randomLines.Length);
                safety++;
            }
        }
        lastIndex = index;

        string line = randomLines[index];

        if (nameText != null)
        {
            nameText.text = speakerName;   // ここに「仲間の名前」が入る
        }


        if (bodyText != null)
        {
            bodyText.text = line;
        }

        if (talkPanel != null)
        {
            talkPanel.SetActive(true);
        }

        isTalking = true;
    }

    void EndTalk()
    {
        if (talkPanel != null)
        {
            talkPanel.SetActive(false);
        }
        isTalking = false;
    }

    void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            isPlayerInRange = true;
        }
    }

    void OnTriggerExit2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            isPlayerInRange = false;
            if (isTalking)
            {
                EndTalk();
            }
        }
    }
}