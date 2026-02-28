using UnityEngine;

public class TitleUIController : MonoBehaviour
{
    [Header("UI")]
    public GameObject titlePanel;      // タイトル画面
    public GameObject tutorialPanel;   // チュートリアル説明パネル

    [Header("シナリオ本体")]
    public ScenarioManager scenarioManager;

    bool inTutorial = false;

    void Start()
    {
        // 最初はタイトルだけ表示
        if (titlePanel != null) titlePanel.SetActive(true);
        if (tutorialPanel != null) tutorialPanel.SetActive(false);

        // 念のため ScenarioManager を自動取得（Inspectorで入れ忘れてても拾えるように）
        if (scenarioManager == null)
        {
            scenarioManager = ScenarioManager.Instance;
        }
    }

    void Update()
    {
        // チュートリアル表示中に Z を押したら → パネルを閉じてシナリオ開始
        if (inTutorial && Input.GetKeyDown(KeyCode.Z))
        {
            Debug.Log("[TitleUI] Z 押下 → チュートリアル終了 & 本編開始");

            if (tutorialPanel != null)
                tutorialPanel.SetActive(false);

            // ここで BeginScenario を呼ぶ
            if (scenarioManager != null)
            {
                scenarioManager.BeginScenario();
            }
            else
            {
                Debug.LogError("[TitleUI] scenarioManager が設定されていません");
            }

            inTutorial = false;
        }
    }

    // NewGame ボタンにこの関数を紐付けする
    public void OnClickNewGame()
    {
        Debug.Log("[TitleUI] NewGame ボタン押下");

        if (titlePanel != null)
            titlePanel.SetActive(false);

        if (tutorialPanel != null)
            tutorialPanel.SetActive(true);

        inTutorial = true;
    }
}