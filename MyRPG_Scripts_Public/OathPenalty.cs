using UnityEngine;

public enum OathTriggerType
{
    AfterTurns,     // 〇ターン後
    OnBattleEnd,    // 戦闘終了時
    OnNextScene     // 次のシーン / 章開始時 など
}

public enum OathPenaltyType
{
    MaxHpDown,
    AttackDown,
    DefenseDown
    // 必要に応じて追加
}

[System.Serializable]
public class OathPenalty
{
    public string name;               // 「誓約：HPを削る」など
    public OathTriggerType trigger;   // いつ発動するか
    public OathPenaltyType penaltyType;  // 何を下げるか
    public int amount;                // 数値（例：-20など）
    public int turnsRemaining;        // AfterTurns 用のカウンター

    public OathPenalty(string name,
                       OathTriggerType trigger,
                       OathPenaltyType penaltyType,
                       int amount,
                       int turnsRemaining = 0)
    {
        this.name = name;
        this.trigger = trigger;
        this.penaltyType = penaltyType;
        this.amount = amount;
        this.turnsRemaining = turnsRemaining;
    }
}
