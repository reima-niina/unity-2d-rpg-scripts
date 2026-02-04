using System.Collections;
using UnityEngine;

public class TitleManager : MonoBehaviour
{
    [Header("UI")]
    public GameObject titleCanvas;          // TitleCanvas（NewGameボタン含む）

    [Header("チュートリアル用カットシーン")]
    public StoryCutscene tutorialCutscene;  // さっき作った TutorialCutscene

    [Header("シナリオ本体")]
    public ScenarioManager scenarioManager; // シーン内の ScenarioManager

    bool isStarting = false;

    //void Start()
    //{
    //    if (BGMManager.Instance != null)
    //    {
    //        BGMManager.Instance.PlayTitleBGM();
    //    }
    //    else
    //    {
    //        Debug.LogWarning("[TitleBGMStarter] BGMManager.Instance が見つかりません");
    //    }
    //}


    // NewGameボタンから呼ぶ用
    public void OnClickNewGame()
    {
        if (isStarting) return;
        StartCoroutine(NewGameFlow());
    }

    private IEnumerator NewGameFlow()
    {
        isStarting = true;

        // ① タイトルを消す
        if (titleCanvas != null)
        {
            titleCanvas.SetActive(false);
        }

        // ② チュートリアル会話を流す
        if (tutorialCutscene != null)
        {
            yield return StartCoroutine(tutorialCutscene.PlayOpening());
        }

        // ③ 本編 Day1 カットシーン開始
        if (scenarioManager != null)
        {
            scenarioManager.BeginScenario();
        }
        else
        {
            Debug.LogWarning("[TitleManager] ScenarioManager がアサインされていません");
        }

        isStarting = false;
    }
}