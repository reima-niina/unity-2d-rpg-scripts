using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GameOverManager : MonoBehaviour
{
    public static GameOverManager Instance;

    [Header("ゲームオーバーUI")]
    public GameObject gameOverPanel;       // GameOverPanel を入れる
    public KeyCode confirmKey = KeyCode.Z; // 押すキー
    public string titleSceneName = "SampleScene"; // タイトルシーンの名前

    bool isShowing = false;

    void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
        }
        else
        {
            Destroy(gameObject);
            return;
        }

        if (gameOverPanel != null)
        {
            gameOverPanel.SetActive(false); // 最初は非表示
        }
    }

    public void ShowGameOver()
    {
        if (isShowing) return;
        isShowing = true;

        // 画面に GameOver パネルを出す
        if (gameOverPanel != null)
        {
            gameOverPanel.SetActive(true);
        }

        if (BGMManager.Instance != null)
        {
            BGMManager.Instance.PlayGameOverBGM();
        }
        if (SEManager.Instance != null)
        {
            SEManager.Instance.PlayGameOver();
        }

        // プレイヤーはもう動かさない
        if (BattleManager.Instance != null &&
            BattleManager.Instance.playerMovement != null)
        {
            BattleManager.Instance.playerMovement.canMove = false;
        }

        // Z入力待ち開始
        StartCoroutine(WaitForInput());
    }

    IEnumerator WaitForInput()
    {
        while (true)
        {
            if (Input.GetKeyDown(confirmKey))
            {
                // タイトルシーンへ
                SceneManager.LoadScene(titleSceneName);
                yield break;
            }
            yield return null;
        }
    }
}