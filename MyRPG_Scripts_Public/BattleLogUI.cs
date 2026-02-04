using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class BattleLogUI : MonoBehaviour
{
    public TMP_Text logText;   // ログ表示用のテキスト
    public int maxLines = 20;  // 最大行数

    public ScrollRect scrollRect;

    public static BattleLogUI Instance { get; private set; }

    private Queue<string> lines = new Queue<string>();

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }
        Instance = this;
    }

    public void AddLog(string message)
    {
        if (string.IsNullOrEmpty(message)) return;

        lines.Enqueue(message);

        while (lines.Count > maxLines)
        {
            lines.Dequeue();
        }

        if (logText != null)
        {
            logText.text = string.Join("\n", lines);
        }

        // 一番下までスクロール
        if (scrollRect != null)
        {
            scrollRect.verticalNormalizedPosition = 0f;
        }

    }

    public void ClearLog()
    {
        lines.Clear();
        if (logText != null)
        {
            logText.text = "";
        }

        if (scrollRect != null)
        {
            scrollRect.verticalNormalizedPosition = 1f;
        }

    }
}
