using UnityEngine;

public class SEManager : MonoBehaviour
{
    public static SEManager Instance { get; private set; }

    public AudioSource seSource;

    [Header("SEƒNƒŠƒbƒv")]
    public AudioClip warpSE;
    public AudioClip buttonClickSE;
    public AudioClip playerAttackSE;
    public AudioClip companionAttackSE;
    public AudioClip enemyAttackSE;
    public AudioClip gameOverSE;

    void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }
        Instance = this;
        DontDestroyOnLoad(gameObject);

        if (seSource == null)
        {
            seSource = gameObject.AddComponent<AudioSource>();
            seSource.loop = false;
        }
    }

    public void PlaySE(AudioClip clip)
    {
        if (clip == null || seSource == null) return;
        seSource.PlayOneShot(clip);
    }

    public void PlayWarp() => PlaySE(warpSE);
    public void PlayClick() => PlaySE(buttonClickSE);
    public void PlayPlayerAttack() => PlaySE(playerAttackSE);
    public void PlayCompanionAttack() => PlaySE(companionAttackSE);
    public void PlayEnemyAttack() => PlaySE(enemyAttackSE);
    public void PlayGameOver() => PlaySE(gameOverSE);
}