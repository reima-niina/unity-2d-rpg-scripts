using UnityEngine;
using TMPro;

public class EndingManager : MonoBehaviour
{
    [Header("エンディング用パネル")]
    public GameObject endingPanel;       // 真っ黒＋テキスト

    public TextMeshProUGUI endingText;   // 後日談テキスト
    public TextMeshProUGUI endLabel;     // END

    void Start()
    {
        if (endingPanel != null)
        {
            // 最初はOFF
            endingPanel.SetActive(false);
        }
    }

    // ラスボス撃破後に BattleManager や BossBattleController から呼ぶ
    public void ShowEnding(string story)
    {
        if (endingPanel != null)
        {
            endingPanel.SetActive(true);
        }

        if (endingText != null)
        {
            endingText.text = story;
        }

        if (endLabel != null)
        {
            endLabel.text = "END";
        }

        // ここで入力停止したいなら、PlayerMovement2D.canMove = false とかもあり
    }
}