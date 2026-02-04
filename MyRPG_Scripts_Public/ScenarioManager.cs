using System.Collections;
using UnityEngine;
using TMPro;

public enum StoryPhase
{
    Day1,               // オープニング（3人会話）
    Day2_TalkVillagers, // 村人に話を聞きに行くフェーズ
    FreeAdventure       // その後の自由行動
}

public class ScenarioManager : MonoBehaviour
{
    public static ScenarioManager Instance;

    [Header("キャラ")]
    public GameObject hero;           // 主人公
    public GameObject friend;         // 親友
    public GameObject heroine;        // ヒロイン
    public PlayerMovement2D playerMove;

    [Header("カメラ")]
    public GameObject fieldCamera;    // ふだんのフィールド用
    public GameObject cutsceneCamera; // カットシーン専用（固定）

    [Header("アニメーター")]
    public Animator heroAnimator;
    public Animator friendAnimator;
    public Animator heroineAnimator;

    [Header("仲間の追従")]
    public CompanionFollow companionFollow;

    [Header("カットシーン")]
    public StoryCutscene day1Cutscene;
    public StoryCutscene day2Cutscene;
    public StoryCutscene afterVillagersCutscene;

    [Header("暗転 & テロップ")]
    public CanvasGroup fadeCanvas;       // 黒幕
    public TextMeshProUGUI dayLabel;     // 「次の日」

    [Header("村人関連")]
    public int totalVillagers = 5;
    private int talkedVillagers = 0;

    private StoryPhase phase = StoryPhase.Day1;

    

    void Awake()
    {
        if (Instance == null) Instance = this;
        else Destroy(gameObject);
    }

    void Start()
    {
        // フェード関係初期化
        if (fadeCanvas != null)
        {
            fadeCanvas.alpha = 0f;
            fadeCanvas.blocksRaycasts = false;
        }

        if (dayLabel != null) dayLabel.gameObject.SetActive(false);

        if (playerMove != null)
        {
            playerMove.enabled = true;      // 念のためON
            playerMove.canMove = false;     // 最初は止めておく（Day1カットシーンのため）
        }

        if (companionFollow != null)
        {
            companionFollow.enabled = true;         // 念のためON
            companionFollow.SetCutsceneMode(true);  // カットシーン中扱いでスタート
        }


        // カメラ初期状態（カットシーンだけ cutsceneCamera を使う）
        if (fieldCamera != null) fieldCamera.SetActive(false);
        if (cutsceneCamera != null) cutsceneCamera.SetActive(true);

        

    }

    public void BeginScenario()
    {
        Debug.Log("[Scenario] BeginScenario が呼ばれました");
        StartCoroutine(MainFlow());

    }

    void UseCutsceneCamera()
    {
        Debug.Log("[Scenario] UseCutsceneCamera");
        if (fieldCamera != null) fieldCamera.SetActive(false);
        if (cutsceneCamera != null) cutsceneCamera.SetActive(true);
    }

    void UseFieldCamera()
    {
        Debug.Log("[Scenario] UseFieldCamera");
        if (cutsceneCamera != null) cutsceneCamera.SetActive(false);
        if (fieldCamera != null) fieldCamera.SetActive(true);
    }

    // メインのシナリオ進行
    IEnumerator MainFlow()
    {
        // Day1 カットシーン開始 
        if (BGMManager.Instance != null)
        {
            BGMManager.Instance.PlayOpeningBGM();
        }
       
        UseCutsceneCamera();

        // プレイヤー＆仲間の操作を止める
        if (playerMove != null)
        {
            playerMove.canMove = false;
            // playerMove.enabled は触らない
        }
        if (companionFollow != null)
        {
            companionFollow.SetCutsceneMode(true);   // 追従止める
        }

        // カットシーン用アニメをトリガー
        if (heroAnimator != null) heroAnimator.SetTrigger("CutsceneTalk");
        if (friendAnimator != null) friendAnimator.SetTrigger("CutsceneTalk");
        if (heroineAnimator != null) heroineAnimator.SetTrigger("CutsceneTalk");

        // Day1 会話
        if (day1Cutscene != null)
        {
            yield return StartCoroutine(day1Cutscene.PlayOpening());
        }

        // 暗転 → 次の日
        yield return StartCoroutine(FadeOut(1.0f));

        if (dayLabel != null)
        {
            dayLabel.text = "次の日";
            dayLabel.gameObject.SetActive(true);
        }

        yield return new WaitForSeconds(1.5f);

        if (dayLabel != null)
        {
            dayLabel.gameObject.SetActive(false);
        }

        // ヒロインを消す
        if (heroine != null)
        {
            Debug.Log("[Scenario] ヒロインを非表示にします: " + heroine.name);
            heroine.SetActive(false);
        }

        yield return StartCoroutine(FadeIn(1.0f));

        // Day2 カットシーン（主人公＋親友）
        // まだカットシーンカメラのまま
        if (day2Cutscene != null)
        {
            yield return StartCoroutine(day2Cutscene.PlayOpening());
        }

        Debug.Log("[Scenario] Day2 Cutscene 終了");

        // ここから自由行動
        UseFieldCamera();

        if (playerMove != null)
        {
            playerMove.canMove = true;
            Debug.Log("[Scenario] playerMove.canMove = TRUE (Day2あと)");
        }

        if (companionFollow != null)
        {
            companionFollow.SetCutsceneMode(false);
        }


        // ここで村BGMを流す（メソッド名は自分の BGMManager に合わせて）
        if (BGMManager.Instance != null)
        {
            BGMManager.Instance.PlayVillageBGM();   // 例：村用BGM
        }


        phase = StoryPhase.Day2_TalkVillagers;
        talkedVillagers = 0;
    }

    // 村人に話しかけ終わったとき呼ぶやつ（NPC側から NotifyTalkedVillager() を呼ぶ）
    public void NotifyTalkedVillager()
    {
        if (phase != StoryPhase.Day2_TalkVillagers) return;

        talkedVillagers++;
        Debug.Log($"[Scenario] Villager talked: {talkedVillagers}/{totalVillagers}");

        if (talkedVillagers >= totalVillagers)
        {
            StartCoroutine(AfterVillagersFlow());
        }
    }

    IEnumerator AfterVillagersFlow()
    {
        phase = StoryPhase.FreeAdventure;

        // もう一度、プレイヤーを止めてカットシーン開始
        if (playerMove != null) playerMove.canMove = false;
        if (companionFollow != null) companionFollow.SetCutsceneMode(true);

        if (afterVillagersCutscene != null)
        {
            yield return StartCoroutine(afterVillagersCutscene.PlayOpening());
        }

        // カットシーン終了 → 再び自由行動
        if (playerMove != null) playerMove.canMove = true;
        if (companionFollow != null) companionFollow.SetCutsceneMode(false);

        Debug.Log("[Scenario] Free adventure unlocked");
    }

    // シンプルなフェード処理
    IEnumerator FadeOut(float duration)
    {
        if (fadeCanvas == null) yield break;

        // 暗転中はクリックをブロック
        fadeCanvas.blocksRaycasts = true;

        float t = 0f;
        while (t < duration)
        {
            t += Time.deltaTime;
            fadeCanvas.alpha = Mathf.Clamp01(t / duration);
            yield return null;
        }
        fadeCanvas.alpha = 1f;

        // フェードが終わったらクリック解放
        fadeCanvas.blocksRaycasts = false;
    }

    IEnumerator FadeIn(float duration)
    {
        if (fadeCanvas == null) yield break;

        float t = 0f;
        while (t < duration)
        {
            t += Time.deltaTime;
            fadeCanvas.alpha = 1f - Mathf.Clamp01(t / duration);
            yield return null;
        }
        fadeCanvas.alpha = 0f;
    }
}