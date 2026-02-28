using UnityEngine;
using TMPro;
using UnityEngine.UI;
using UnityEngine.Networking;
using System.Collections;

public class SimpleAINPCDialogue : MonoBehaviour
{
    [Header("UI参照")]
    public GameObject dialogPanel;
    public TMP_Text nameText;
    public TMP_Text bodyText;

    // ここを GameObject にして何でもドラッグできるようにする
    public GameObject inputFieldObject;
    public GameObject sendButtonObject;

    // 実際に使うコンポーネント
    private TMP_InputField inputField;
    private Button sendButton;

    [Header("NPC設定")]
    public string npcName = "博識な村人";

    [Header("AI クライアント")]
    public ChatGPTClient chatClient;


    bool playerInRange = false;
    bool isOpen = false;

    void Start()
    {
        if (dialogPanel != null)
            dialogPanel.SetActive(false);

        // InputField を取得
        if (inputFieldObject != null)
            inputField = inputFieldObject.GetComponent<TMP_InputField>();

        if (sendButtonObject != null)
            sendButton = sendButtonObject.GetComponent<Button>();


    }

    void Update()
    {
        if (!playerInRange) return;

        if (Input.GetKeyDown(KeyCode.Z))
        {
            if (!isOpen)
            {
                OpenDialog();
            }
            else
            {
                CloseDialog();
            }
        }
    }

    void OpenDialog()
    {
        isOpen = true;

        if (dialogPanel != null)
            dialogPanel.SetActive(true);

        if (nameText != null)
            nameText.text = npcName;

        if (bodyText != null)
            bodyText.text = "何か話しかけてみよう。";

        if (inputField != null)
        {
            inputField.text = "";
            inputField.ActivateInputField();
        }
    }

    void CloseDialog()
    {
        isOpen = false;

        if (dialogPanel != null)
            dialogPanel.SetActive(false);
    }

    // ボタンから呼ぶ用
    public void OnClickSend()
    {

        if (inputField == null)
        {
            Debug.LogWarning("inputField が null です。inputFieldObject と InputField コンポーネントを確認してね。");
            return;
        }

        if (bodyText == null)
        {
            Debug.LogError("SimpleNPCDialogueUI: bodyText が null です");
            return;
        }


        string msg = inputField.text;
        Debug.Log($"入力内容: 『{msg}』");

        if (string.IsNullOrWhiteSpace(msg))
        {
            bodyText.text = "何か話してから送信してね。";
            return;
        }

        inputField.text = "";

        if (chatClient == null)
        {
            Debug.LogError("SimpleNPCDialogueUI: chatClient が null です");
            bodyText.text = $"今日はちょっと頭が回らないみたいだ…";
            return;
        }


        // 読み込みの表示
        bodyText.text = $"ふむふむ。";



        // ここで ChatGPT に投げる
        StartCoroutine(chatClient.SendChat(msg, reply =>
        {
            if (bodyText == null)
            {
                Debug.LogWarning("SimpleNPCDialogueUI: 返事を表示しようとしたら bodyText が null だった");
                return;
            }

            bodyText.text = $"{reply}";
        }));

    }

    void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
            playerInRange = true;
    }

    void OnTriggerExit2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            playerInRange = false;
            if (isOpen)
                CloseDialog();
        }
    }
}