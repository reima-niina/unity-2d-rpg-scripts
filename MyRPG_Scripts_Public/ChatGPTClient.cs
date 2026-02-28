using System;
using System.Collections;
using System.Text;
using UnityEngine;
using UnityEngine.Networking;

public class ChatGPTClient : MonoBehaviour
{
    [Header("OpenAI 設定")]
    [SerializeField] private string apiKey;                   // Inspectorで入れる
    [SerializeField] private string model = "gpt-4o-mini";    // 好きなモデル名

    private bool isRequesting = false;   // 今リクエスト中かどうか

    private const string ChatUrl = "https://api.openai.com/v1/chat/completions";

    // リクエスト用クラス
    [Serializable]
    class ChatMessage
    {
        public string role;
        public string content;
    }

    [Serializable]
    class ChatRequest
    {
        public string model;
        public ChatMessage[] messages;
        public float temperature = 0.7f;
    }

    // レスポンス用クラス
    [Serializable]
    class ChatChoiceMessage
    {
        public string role;
        public string content;
    }

    [Serializable]
    class ChatChoice
    {
        public int index;
        public ChatChoiceMessage message;
    }

    [Serializable]
    class ChatResponse
    {
        public ChatChoice[] choices;
    }

    public IEnumerator SendChat(string userMessage, Action<string> onCompleted)
    {
        // 連打ガード
        if (isRequesting)
        {
            onCompleted?.Invoke("今考え中だから、ちょっと待って");
            yield break;
        }

        isRequesting = true;  // 「リクエスト中」フラグON

        if (string.IsNullOrEmpty(apiKey))
        {
            Debug.LogError("ChatGPTClient: APIキーが設定されていません");
            onCompleted?.Invoke("（APIキーが未設定です）");
            isRequesting = false;
            yield break;
        }

        // 送るメッセージ（system + user）
        var req = new ChatRequest
        {
            model = model,
            messages = new[]
            {
                new ChatMessage
                {
                    role = "system",
                    content = "あなたはRPGの危険な森に住む人です。日本語のタメ口で2〜3文くらいの短い返事をしてください。"
                },
                new ChatMessage
                {
                    role = "user",
                    content = userMessage
                }
            }
        };

        string json = JsonUtility.ToJson(req);
        byte[] bodyRaw = Encoding.UTF8.GetBytes(json);

        using (var www = new UnityWebRequest(ChatUrl, "POST"))
        {
            www.uploadHandler = new UploadHandlerRaw(bodyRaw);
            www.downloadHandler = new DownloadHandlerBuffer();
            www.SetRequestHeader("Content-Type", "application/json");
            www.SetRequestHeader("Authorization", "Bearer " + apiKey);

            Debug.Log("OpenAI に送信: " + json);

            // 送信して待つ
            yield return www.SendWebRequest();

            // 通信エラー系
            if (www.result != UnityWebRequest.Result.Success)
            {
                Debug.LogError("ChatGPTClient エラー: " + www.error + "\n" + www.downloadHandler.text);

                if (www.responseCode == 429)
                {
                    onCompleted?.Invoke("ごめん、今日はちょっと喋りすぎて怒られてる…少し時間をあけてくれない？");
                }
                else
                {
                    onCompleted?.Invoke("……うまく返事ができなかったみたいだ");
                }

                isRequesting = false;
                yield break;
            }

            // 正常時：レスポンスJSONを読む
            string responseJson = www.downloadHandler.text;
            Debug.Log("OpenAI からのレスポンス: " + responseJson);

            ChatResponse res = null;
            try
            {
                res = JsonUtility.FromJson<ChatResponse>(responseJson);
            }
            catch (Exception e)
            {
                Debug.LogError("レスポンスのパースに失敗: " + e);
                onCompleted?.Invoke("……（意味不明な書き物を渡された）");
                isRequesting = false;
                yield break;
            }

            if (res == null || res.choices == null || res.choices.Length == 0 || res.choices[0].message == null)
            {
                onCompleted?.Invoke("……（何も答えてくれない）");
                isRequesting = false;
                yield break;
            }

            string content = res.choices[0].message.content.Trim();
            onCompleted?.Invoke(content);

            isRequesting = false;   // 最後にOFF
        }
    }
}