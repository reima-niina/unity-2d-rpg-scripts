using UnityEngine;

public class BGMManager : MonoBehaviour
{
    public static BGMManager Instance { get; private set; }

    [Header("BGM再生用AudioSource")]
    public AudioSource bgmSource;

    [Header("各シーン用BGM")]
    public AudioClip titleBGM;
    public AudioClip openingBGM;    // 最初のカットシーン
    public AudioClip villageBGM;    // 村
    public AudioClip forestBGM;     // 森
    public AudioClip fieldBGM;      // フィールド
    public AudioClip castleBGM;     // 城の中
    public AudioClip battleBGM;     // 通常戦闘
    public AudioClip bossBattleBGM; // ボス戦
    public AudioClip gameOverBGM;   // ゲームオーバー
    public AudioClip bossClearBGM;  // ボス撃破後のBGM

    // 戦闘に入る前に鳴っていたBGMを保存しておく
    private AudioClip beforeBattleBGM;

    void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }
        Instance = this;
        DontDestroyOnLoad(gameObject);

        if (bgmSource == null)
        {
            bgmSource = gameObject.AddComponent<AudioSource>();
            bgmSource.loop = true;
        }
    }

    void PlayBGM(AudioClip clip, bool loop = true)
    {
        if (clip == null || bgmSource == null) return;

        // すでに同じ曲を再生中ならかけ直さない
        if (bgmSource.clip == clip && bgmSource.isPlaying) return;

        bgmSource.loop = loop;
        bgmSource.clip = clip;
        bgmSource.Play();
    }

    public void StopBGM()
    {
        if (bgmSource != null) bgmSource.Stop();
    }

    public void PlayBossClearBGM() => PlayBGM(bossClearBGM);

    // 通常エリアBGM
    public void PlayFieldBGM() => PlayBGM(fieldBGM);
    public void PlayTitleBGM() => PlayBGM(titleBGM);
    public void PlayOpeningBGM() => PlayBGM(openingBGM);
    public void PlayVillageBGM() => PlayBGM(villageBGM);
    public void PlayForestBGM() => PlayBGM(forestBGM);
    public void PlayCastleBGM() => PlayBGM(castleBGM);

    // 戦闘BGM（ここで直前の曲を記録）
    public void PlayBattleBGM()
    {
        RememberCurrentBGM();
        PlayBGM(battleBGM);
    }

    public void PlayBossBattleBGM()
    {
        RememberCurrentBGM();
        PlayBGM(bossBattleBGM);
    }

    // ゲームオーバー
    public void PlayGameOverBGM() => PlayBGM(gameOverBGM, loop: false);

    // 戦闘前のBGMを覚えておき → 戦闘後に戻す

    // 今鳴っているBGMを保存
    private void RememberCurrentBGM()
    {
        if (bgmSource != null && bgmSource.clip != null)
        {
            beforeBattleBGM = bgmSource.clip;
        }
        else
        {
            beforeBattleBGM = null;
        }
    }

    // 戦闘が終わったら呼ばれる想定
    public void RestoreBGMBeforeBattle()
    {
        if (beforeBattleBGM != null)
        {
            PlayBGM(beforeBattleBGM);
        }
        else
        {
            // 念のため何も覚えてなかったとき用
            PlayFieldBGM();
        }
    }
}