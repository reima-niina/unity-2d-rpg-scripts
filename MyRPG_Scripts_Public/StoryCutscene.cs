using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

// 1行ぶんのセリフデータ
[System.Serializable]
public class StoryLine
{
    public string speakerName;          // 話している人の名前
    [TextArea(2, 5)]
    public string text;                 // セリフ本文
}

// 実際にオブジェクトにアタッチするカットシーン本体
public class StoryCutscene : MonoBehaviour
{
    [Header("UI 参照")]
    public GameObject storyPanel;           // 会話ウィンドウ（パネル）
    public TextMeshProUGUI nameText;        // 名前表示用
    public TextMeshProUGUI bodyText;        // セリフ表示用

    [Header("オープニングのセリフ一覧")]
    public List<StoryLine> lines = new List<StoryLine>();

    [Header("送りキー")]
    public KeyCode nextKey = KeyCode.Z;


    private bool isPlaying = false;

    private void Start()
    {
        // 最初は非表示
        //if (storyPanel != null)
        //{
        //    storyPanel.SetActive(false);
        //}

    }

    // オープニング再生本体
    public IEnumerator PlayOpening()
    {
        if (isPlaying) yield break;
        isPlaying = true;

        // ここでは「動きを止めない」ので canMove は触らない

        if (storyPanel != null)
        {
            storyPanel.SetActive(true);
        }

        // 1行ずつ表示
        foreach (var line in lines)
        {

            if (nameText != null)
            {
                nameText.text = line.speakerName;
            }

            if (bodyText != null)
            {
                bodyText.text = line.text;
            }

            // Zキーが押されるまで待つ
            yield return WaitForNextKey();
        }

        // 終了処理
        if (storyPanel != null)
        {
            storyPanel.SetActive(false);
        }

        isPlaying = false;
    }

    // 次のセリフへ進むのを待つコルーチン
    private IEnumerator WaitForNextKey()
    {
        // 前フレームの押しっぱなしを拾わないよう一瞬待つ
        yield return null;

        while (true)
        {
            if (Input.GetKeyDown(nextKey))
            {
                break;
            }
            yield return null;
        }
    }
}