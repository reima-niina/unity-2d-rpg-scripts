using System.Collections;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using static AreaBGMTrigger;

public class BattleManager : MonoBehaviour
{
    public static BattleManager Instance { get; private set; }

    [Header("参照")]
    public PlayerMovement2D playerMovement;
    public CharacterStats playerStats;

    [Header("今回バトル中の敵")]
    public CharacterStats enemyStats;      // 今戦っている敵
    public GameObject fieldEnemySymbol;    // フィールド上の敵オブジェクト

    [Header("バトル用アニメーション")]
    public Animator enemyBattleAnimator;

    [Header("バトルUI")]
    public GameObject battleUI;
    public Image enemyBattleImage;         // バトル画面の敵Image
    public TMP_Text enemyNameText;         // 敵の名前表示

    [Header("味方（仲間）")]
    public CharacterStats companionStats;        // 仲間のステータス

    [Header("味方UI（任意）")]
    public Image companionBattleImage;           // 仲間のアイコン
    public TMP_Text companionNameText;           // 仲間の名前表示
    public TMP_Text companionHpText;             // 仲間HP表示
    public TMP_Text playerHpText;                // 主人公HP表示
    public TMP_Text enemyHpText;                 // 敵HP表示

    [Header("HPバーUI")]
    public HPBarUI playerHpBar;      // プレイヤー用
    public HPBarUI companionHpBar;   // 仲間用
    public HPBarUI enemyHpBar;       // 敵用

    public GameObject companionUIRoot;           // 仲間のUIをまとめてON/OFFしたいとき

    [Header("ラスボス撃破後の演出")]
    public StoryCutscene lastBossAfterBattleCutscene;  // 勝利後の会話
    public GameObject lastBossFieldObject;             // フィールド上のラスボス本体
    public StoryCutscene endingCutscene;

    [Header("エンディング後のシーン遷移")]
    public string titleSceneName = "TitleScene";

    // 内部フラグ
    private bool inBattle = false;

    // 外から「今バトル中か」を見たいとき用
    public bool InBattle => inBattle;

    // 1ターン処理中かどうか（ボタン連打でバグらないように）
    private bool isProcessingTurn = false;

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }
        Instance = this;
    }

    private void Start()
    {
        if (playerHpBar != null && playerStats != null)
        {
            playerHpBar.SetTarget(playerStats);
        }

        if (companionHpBar != null && companionStats != null)
        {
            companionHpBar.SetTarget(companionStats);
        }
    }

    // メインの StartBattle（フィールドシンボル付き）
    public void StartBattle(CharacterStats enemy, GameObject enemySymbol)
    {
        if (inBattle) return;
        inBattle = true;

        // どの敵と戦うかをここで決定
        enemyStats = enemy;
        fieldEnemySymbol = enemySymbol;

        // ラスボスかどうかで分岐
        bool isLastBossBattle = (enemyStats != null && enemyStats.isLastBoss);

        // BGM をボスかどうかで切り替え
        if (BGMManager.Instance != null)
        {
            if (isLastBossBattle)
            {
                // ラスボス戦用BGM
                BGMManager.Instance.PlayBossBattleBGM();
            }
            else
            {
                // ふつうの戦闘BGM
                BGMManager.Instance.PlayBattleBGM();
            }
        }

        // 雑魚・中ボスだけフィールドのシンボルを消す
        if (!isLastBossBattle && fieldEnemySymbol != null)
        {
            fieldEnemySymbol.SetActive(false);
        }

        // 敵HPを全回復
        if (enemyStats != null)
        {
            enemyStats.ResetHP();   // 毎回HPを全回復
            UpdateEnemyHpUI();
        }

        // ここで敵HPバーのターゲットをセット
        if (enemyHpBar != null && enemyStats != null)
        {
            enemyHpBar.SetTarget(enemyStats);
        }

        // プレイヤーHPバーも念のため更新
        if (playerHpBar != null && playerStats != null)
        {
            playerHpBar.SetTarget(playerStats);
        }

        if (companionHpBar != null && companionStats != null)
        {
            companionHpBar.SetTarget(companionStats);
        }

        // 主人公・仲間のHP表示を整える
        if (playerStats != null)
        {
            if (playerStats.currentHP > playerStats.maxHP)
                playerStats.currentHP = playerStats.maxHP;
            UpdatePlayerHpUI();
        }

        if (companionStats != null)
        {
            if (companionStats.currentHP > companionStats.maxHP)
                companionStats.currentHP = companionStats.maxHP;
            UpdateCompanionHpUI();
        }

        // 仲間UIのON/OFF
        if (companionUIRoot != null)
        {
            companionUIRoot.SetActive(companionStats != null);
        }

        // バトルUIを表示
        if (battleUI != null)
        {
            battleUI.SetActive(true);
        }

        // 敵の画像と名前をUIに反映
        if (enemyBattleImage != null && enemyStats != null)
        {
            enemyBattleImage.sprite = enemyStats.battleSprite;
            enemyBattleImage.enabled = true;
            enemyBattleImage.preserveAspect = true;

            // スプライト本来のサイズに合わせる
            enemyBattleImage.SetNativeSize();

            // 敵ごとの倍率をかける
            RectTransform rt = enemyBattleImage.rectTransform;
            rt.localScale = Vector3.one * enemyStats.battleUIScale;

        }

        if (enemyNameText != null && enemyStats != null)
        {
            enemyNameText.text = enemyStats.characterName;
        }


        // ログ
        BattleLogUI.Instance?.ClearLog();
        if (enemyStats != null)
        {
            BattleLogUI.Instance?.AddLog($"{enemyStats.characterName} が あらわれた！");
        }

        // プレイヤー移動停止
        if (playerMovement != null)
        {
            playerMovement.StopMovement();
        }
    }

    public void OnAttackButton()
    {
        if (!inBattle) return;
        if (isProcessingTurn) return;

        StartCoroutine(PlayerPhase());
    }

    private IEnumerator PlayerPhase()
    {
        isProcessingTurn = true;

        // ▶ 1. プレイヤー攻撃
        yield return PlayerAttack();

        if (enemyStats == null || enemyStats.IsDead())
        {
            EndBattle();
            isProcessingTurn = false;
            yield break;
        }

        // ▶ 2. 仲間攻撃（仲間がいて生きているときだけ）
        if (companionStats != null && !companionStats.IsDead())
        {
            yield return CompanionAttack();

            if (enemyStats == null || enemyStats.IsDead())
            {
                EndBattle();
                isProcessingTurn = false;
                yield break;
            }
        }

        // ▶ 3. 敵ターン
        yield return EnemyTurn();

        // プレイヤー死亡チェック
        if (playerStats != null && playerStats.IsDead())
        {
            BattleLogUI.Instance?.AddLog(
                $"{playerStats.characterName} は たおれてしまった…"
            );

            // まずバトル状態を終わらせる
            inBattle = false;

            // バトルUIを消す
            if (battleUI != null)
            {
                battleUI.SetActive(false);
            }

            // BGM
            if (BGMManager.Instance != null)
            {
                BGMManager.Instance.PlayFieldBGM();
            }

            // ゲームオーバー画面へ
            if (GameOverManager.Instance != null)
            {
                GameOverManager.Instance.ShowGameOver();
            }

            isProcessingTurn = false;
            yield break;

        }

        isProcessingTurn = false;
    }

    // 禁術のあとに呼ぶ用：仲間→敵ターンだけ回す
    public void StartPostOathTurn()
    {
        if (!inBattle) return;
        if (isProcessingTurn) return;

        StartCoroutine(PostOathPhase());
    }

    private IEnumerator PostOathPhase()
    {
        isProcessingTurn = true;

        // ▶ 1. 仲間攻撃（生きているときだけ）
        if (companionStats != null && !companionStats.IsDead())
        {
            yield return CompanionAttack();

            // ここで敵が死んだらバトル終了
            if (enemyStats == null || enemyStats.IsDead())
            {
                EndBattle();
                isProcessingTurn = false;
                yield break;
            }
        }

        // ▶ 2. 敵ターン
        yield return EnemyTurn();

        // プレイヤーが死んだらゲームオーバー扱い
        if (playerStats != null && playerStats.IsDead())
        {
            BattleLogUI.Instance?.AddLog(
                $"{playerStats.characterName} は たおれてしまった…"
            );
            EndBattle();
            isProcessingTurn = false;
            yield break;
        }

        isProcessingTurn = false;
    }
    private IEnumerator PlayerAttack()
    {
        if (playerStats == null || enemyStats == null) yield break;

        int raw = playerStats.attack;
        int damage = Mathf.Max(raw - enemyStats.defense, 1);

        enemyStats.TakeRawDamage(damage);
        UpdateEnemyHpUI();

        BattleLogUI.Instance?.AddLog(
            $"{playerStats.characterName} の こうげき！ " +
            $"{enemyStats.characterName} に {damage} のダメージ！"
        );

        yield return new WaitForSeconds(0.3f);

        SEManager.Instance?.PlayPlayerAttack();

        if (enemyStats.IsDead())
        {
            BattleLogUI.Instance?.AddLog($"{enemyStats.characterName} を たおした！");

            // 主人公と仲間の両方にキルを加算
            if (playerStats != null)
            {
                playerStats.RegisterKill();
            }

            if (companionStats != null)
            {
                companionStats.RegisterKill();
            }

        }
    }

    private IEnumerator CompanionAttack()
    {
        if (companionStats == null || enemyStats == null) yield break;
        if (companionStats.IsDead()) yield break;

        int raw = companionStats.attack;
        int damage = Mathf.Max(raw - enemyStats.defense, 1);

        enemyStats.TakeRawDamage(damage);
        UpdateEnemyHpUI();

        BattleLogUI.Instance?.AddLog(
            $"{companionStats.characterName} の こうげき！ " +
            $"{enemyStats.characterName} に {damage} のダメージ！"
        );

        yield return new WaitForSeconds(0.3f);

        SEManager.Instance?.PlayCompanionAttack();

        if (enemyStats.IsDead())
        {
            BattleLogUI.Instance?.AddLog($"{enemyStats.characterName} を たおした！");

            // ここでも両方
            if (playerStats != null)
            {
                playerStats.RegisterKill();
            }

            if (companionStats != null)
            {
                companionStats.RegisterKill();
            }

        }
    }

    private CharacterStats ChooseTarget()
    {
        bool playerAlive = (playerStats != null && !playerStats.IsDead());
        bool companionAlive = (companionStats != null && !companionStats.IsDead());

        // 仲間がいない or 死んでる → プレイヤー一択
        if (!companionAlive && playerAlive)
            return playerStats;

        // プレイヤーが死んでて仲間だけ生きてる → 仲間一択
        if (!playerAlive && companionAlive)
            return companionStats;

        // 両方生きてる → ランダム
        if (playerAlive && companionAlive)
        {
            int r = Random.Range(0, 100);
            return (r < 50) ? playerStats : companionStats;
        }

        // 全滅（ゲームオーバー）→ null
        return null;

        
    }
    private IEnumerator EnemyTurn()
    {
        
        if (enemyStats == null || enemyStats.IsDead()) yield break;

        BattleLogUI.Instance?.AddLog($"{enemyStats.characterName} の こうげき！ ");


        // 敵の攻撃アニメ
        if (enemyBattleAnimator != null)
        {
            enemyBattleAnimator.SetTrigger("Attack");
        }

        yield return new WaitForSeconds(1.0f);

        SEManager.Instance?.PlayEnemyAttack();

        // 攻撃対象を選ぶ
        CharacterStats target = ChooseTarget();
        if (target == null)
        {
            // 全員死んでる
            yield break;
        }

        int raw = enemyStats.attack;
        int damage = Mathf.Max(raw - target.defense, 1);

        target.TakeRawDamage(damage);

        // どっちを殴ったかで HP 表示を更新
        if (target == playerStats)
        {
            UpdatePlayerHpUI();
        }
        else if (target == companionStats)
        {
            UpdateCompanionHpUI();
        }

        BattleLogUI.Instance?.AddLog($"{target.characterName} は {damage} のダメージを うけた！");

        if (target == playerStats && playerStats.IsDead())
        {
            Debug.Log("[BattleManager] 主人公死亡 → GameOver へ");

            // バトル終了扱いにする
            inBattle = false;

            // バトルUIを閉じる
            if (battleUI != null)
                battleUI.SetActive(false);

            // 主人公は動かさない
            if (playerMovement != null)
                playerMovement.canMove = false;

            // GameOverManager を呼ぶ
            if (GameOverManager.Instance != null)
            {
                GameOverManager.Instance.ShowGameOver();
            }
            else
            {
                Debug.LogWarning("GameOverManager.Instance が見つかりません");
            }

            yield break;   // ここで敵ターン終わり
        }


        yield return new WaitForSeconds(0.3f);

    }

    private void UpdatePlayerHpUI()
    {
        if (playerHpText != null && playerStats != null)
        {
            playerHpText.text = $"{playerStats.currentHP} / {playerStats.maxHP}";
        }
    }

    private void UpdateCompanionHpUI()
    {
        if (companionHpText != null && companionStats != null)
        {
            companionHpText.text = $"{companionStats.currentHP} / {companionStats.maxHP}";
        }
    }

    private void UpdateEnemyHpUI()
    {
        if (enemyHpText != null && enemyStats != null)
        {
            enemyHpText.text = $"{enemyStats.currentHP} / {enemyStats.maxHP}";
        }
    }
    // ボス用などシンボル無し用の簡易版 StartBattle
    // BossBattleController からはこっちを呼ぶ
    
    public void StartBattle(CharacterStats enemy)
    {
        StartBattle(enemy, null);
    }

    // バトル終了
    public void EndBattle()
    {
        if (!inBattle) return;

        // ラスボスかつ倒しているかどうか
        bool isLastBossBattle =
            (enemyStats != null && enemyStats.isLastBoss && enemyStats.IsDead());

        inBattle = false;

        // バトルUIを消す
        if (battleUI != null)
        {
            battleUI.SetActive(false);
        }

        // 通常バトルはここでフィールドBGM＋移動再開
        if (!isLastBossBattle)
        {
            // 通常の戦闘：戦闘前に鳴っていたBGMを復帰
            if (BGMManager.Instance != null)
            {
                BGMManager.Instance.RestoreBGMBeforeBattle();
            }


            if (playerMovement != null)
            {
                playerMovement.ResumeMovement();
            }
        }

        else
        {
            // ラスボスは専用フローへ
            StartCoroutine(LastBossEndingFlow());
        }

    }

    private IEnumerator LastBossEndingFlow()
    {
        // ここでボス撃破用BGMに切り替え
        if (BGMManager.Instance != null)
        {
            BGMManager.Instance.PlayBossClearBGM();
        }



        // 念のため、プレイヤー操作を少し制御
        if (playerMovement != null)
        {
            playerMovement.canMove = false;
        }

        // ① 勝利後の会話
        if (lastBossAfterBattleCutscene != null)
        {
            yield return StartCoroutine(lastBossAfterBattleCutscene.PlayOpening());
        }

        // ② ラスボスのオブジェクトを消す
        if (lastBossFieldObject != null)
        {
            lastBossFieldObject.SetActive(false);
        }

        // ③ 画面を真っ黒にする
        if (ScreenFader.Instance != null)
        {
            // ここでフェードアウトして真っ黒のままにしておく
            yield return ScreenFader.Instance.FadeOut(1.0f);
        }

        // ④ 黒背景のまま後日談やENDを表示
        if (endingCutscene != null)
        {
            yield return StartCoroutine(endingCutscene.PlayOpening());
        }

        // ⑤ END を出し終わったあと、Z入力待ち
        // 直前にZを押していた可能性があるので1フレーム待ってから
        yield return null;
        yield return new WaitUntil(() => Input.GetKeyDown(KeyCode.Z));

        // ⑥ タイトルシーンに戻る
        if (!string.IsNullOrEmpty(titleSceneName))
        {
            SceneManager.LoadScene(titleSceneName);
        }
        else
        {
            Debug.LogWarning("[BattleManager] titleSceneName が設定されていません");
        }

    }
    // フィールド敵を復活させたいならここで true に戻す
    // if (fieldEnemySymbol != null) fieldEnemySymbol.SetActive(true);
}
