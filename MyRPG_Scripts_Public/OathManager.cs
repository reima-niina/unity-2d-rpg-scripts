using UnityEngine;

public class OathManager : MonoBehaviour
{
    public static OathManager Instance { get; private set; }

    // 誓約の対象（主人公）
    public CharacterStats targetCharacter;

    // 現在の誓約（継続状態）
    public OathType currentOath = OathType.None;

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }
        Instance = this;
        DontDestroyOnLoad(gameObject);  // シーンまたぎで残す
    }

    /// <summary>
    /// 誓約を設定して、キャラに適用する
    /// </summary>
    public void SetOath(OathType oath)
    {
        currentOath = oath;

        if (targetCharacter != null)
        {
            targetCharacter.ApplyOath(oath);
        }

        Debug.Log($"[OathManager] 誓約を {oath} に設定");
    }

    /// <summary>
    /// 誓約を解除したいとき用（必要なら）
    /// </summary>
    public void ClearOath()
    {
        currentOath = OathType.None;

        if (targetCharacter != null)
        {
            targetCharacter.ApplyOath(OathType.None);
        }

        Debug.Log("[OathManager] 誓約を解除しました");
    }
}
