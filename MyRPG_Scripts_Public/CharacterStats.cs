using UnityEngine;
public enum OathType
{
    None,           // 誓約なし
    MaxHPMinus20,   // 最大HP -20 の誓約
    AttackMinus5,   // 攻撃力 -5 の誓約
    DefenseMinus5   // 防御力 -5の誓約
}

public class CharacterStats : MonoBehaviour
{
    [Header("基本情報")]
    public string characterName = "NoName";

    [Header("キャラクター種別")]
    public bool isCompanion = false;   // 仲間には true を付ける
    public bool isPlayer = false;      // 主人公にもフラグ付ける

    [Header("レベル関係")]
    public int level = 1;
    public int killsToNextLevel = 5;   // 5体倒したらレベルアップ
    public int currentKillCount = 0;   // 倒した数カウント

    [Header("レベルアップ演出")]
    public AudioClip levelUpSE;
    public ParticleSystem levelUpEffectPrefab; // 画面に出すエフェクト
    public AudioSource sfxSource;

    [Header("素のステータス（レベルアップで増える値）")]
    public int baseMaxHP = 100;
    public int baseAttack = 10;
    public int baseDefense = 0;

    [Header("レベルアップ増加量")]
    public int hpPerLevel = 12;  // 1レベルごとに増えるHP
    public int attackPerLevel = 3;   // 1レベルごとに増える攻撃
    public int defensePerLevel = 1;  // 1レベルごとに増える防御

    [Header("ボス設定")]
    public bool isLastBoss = false;

    [Header("バトルUI用見た目")]
    public Sprite battleSprite;

    [Range(0.5f, 3f)]
    public float battleUIScale = 1f;

    [Header("現在のステータス（実際に戦闘で使う値）")]
    public int maxHP;
    public int currentHP;
    public int attack;
    public int defense;

    [Header("誓約による累積ペナルティ")]
    public int oathMaxHpPenalty = 0;   // ここが -20, -40, -60... と増えていく
    public int oathAttackPenalty = 0;  // ここが -5, -10, -15... と増えていく
    public int oathDefensePenalty = 0;

    public OathType lastOath = OathType.None;

   
    public void ApplyOath(OathType oath)
    {
        lastOath = oath;

        switch (oath)
        {
            case OathType.MaxHPMinus20:
                // 毎回 -20 ずつ足していく（-20, -40, -60...）
                oathMaxHpPenalty -= 20;
                break;

            case OathType.AttackMinus5:
                // 毎回 -5 ずつ足していく（-5, -10, -15...）
                oathAttackPenalty -= 5;
                break;

            case OathType.DefenseMinus5:
                oathDefensePenalty -= 5;
                break;

            case OathType.None:
                // 誓約リセットしたくなったら、ここで 0 に戻す処理を書く
                
                break;
        }

        RecalculateStats();

        Debug.Log(
            $"[{characterName}] 誓約 {oath} を追加適用: " +
            $"maxHP={maxHP} (base {baseMaxHP}, penalty {oathMaxHpPenalty}), " +
            $"atk={attack} (base {baseAttack}, penalty {oathAttackPenalty})"
        );
    }
    
    private void Awake()
    {
        // 初期化：最初は「素のステータス」をそのまま使う
        if (maxHP <= 0) maxHP = baseMaxHP;
        if (attack == 0) attack = baseAttack;
        if (defense == 0) defense = baseDefense;
        if (currentHP <= 0 || currentHP > maxHP) currentHP = maxHP;

        RecalculateStats();
    }

    // 誓約やレベルアップ後に呼ぶ
    public void RecalculateStats()
    {
        // ここで「素のステータス ＋ 誓約ペナルティ」を合成する
        maxHP = Mathf.Max(1, baseMaxHP + oathMaxHpPenalty);
        attack = Mathf.Max(0, baseAttack + oathAttackPenalty);
        defense = Mathf.Max(0, baseDefense + oathDefensePenalty);

        if (currentHP > maxHP)
        {
            currentHP = maxHP;
        }
    }

    
    public void TakeRawDamage(int damage)
    {
        currentHP -= damage;
        if (currentHP < 0) currentHP = 0;

        if (currentHP == 0)
        {
            OnDead();   
        }

    }

    // 死亡時の共通処理
    void OnDead()
    {
        // ① まず「仲間かどうか」を CompanionLifeVisual の有無で判定
        var life = GetComponent<CompanionLifeVisual>();
        if (life != null)
        {
            Debug.Log($"[CharacterStats] {characterName} は仲間なので Destroy せず棺桶に変更");
            life.SwitchToCoffin();
            return;  // ここで終了、 Destroy はしない
        }


        // ラスボスだけはここで消さない（BossBattleController に任せる）
        if (isLastBoss)
        {
            Debug.Log($"[CharacterStats] {characterName} はラスボスなので Destroy しない");
            return;
        }

        // それ以外の敵は今まで通りここで消す
        Debug.Log($"[CharacterStats] {characterName} は倒されたので Destroy");
        Destroy(gameObject);
    }

    public bool IsDead()
    {
        return currentHP <= 0;
    }

    public void ResetHP()
    {
        currentHP = maxHP;
    }

    // 敵を倒したときに呼ぶ
    public void RegisterKill()
    {
        currentKillCount++;

        if (currentKillCount >= killsToNextLevel)
        {
            currentKillCount = 0;
            LevelUp();
        }
    }

    // レベルアップ処理
    public void LevelUp()
    {
        level++;

        // レベルアップ時に「素のステータス」を強化
        baseMaxHP += hpPerLevel;
        baseAttack += attackPerLevel;
        baseDefense += defensePerLevel;


        RecalculateStats();

        // ログに出す
        BattleLogUI.Instance?.AddLog(
            $"{characterName} は レベル {level} に あがった！ HPと攻撃力が少し 上がった。"
        );

        // SE
        if (sfxSource != null && levelUpSE != null)
        {
            sfxSource.PlayOneShot(levelUpSE);
        }

        // エフェクト
        if (levelUpEffectPrefab != null)
        {
            Instantiate(
                levelUpEffectPrefab,
                transform.position,
                Quaternion.identity
            );
        }


        Debug.Log(
            $"[LevelUp] {characterName} Lv{level}: baseMaxHP={baseMaxHP}, baseAttack={baseAttack}"
        );
    }
}

