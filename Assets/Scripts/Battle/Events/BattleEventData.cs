using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// One entry in BattleEventData.StartStatuses. Applies a status effect to a
/// specific enemy in the encounter at the start of the battle.
/// </summary>
[System.Serializable]
public struct StartStatus
{
    [Tooltip("The status effect asset to apply.")]
    public StatusEffect Status;
    [Tooltip("Index into the Encounter.Enemies list (0 = first enemy).")]
    public int EnemyIndex;
    [Tooltip("Amplifier/duration passed to GainStatusEffect.")]
    public int Amplifier;
}

/// <summary>
/// EventData for a battle-style event: spawns an Encounter's enemies in the
/// Level scene, optionally applies start statuses and a turn-limit flee,
/// and grants a configurable reward when the last enemy is defeated.
///
/// No per-event script is required — BattleEvent drives all encounters of
/// this type from this asset alone.
/// </summary>
[CreateAssetMenu(fileName = "New Battle Event", menuName = "Events/Battle Event")]
public class BattleEventData : EventData
{
    [Tooltip("The encounter to fight (enemy chain, dialogue, timing).")]
    public Encounter Encounter;

    [Tooltip("Status effects applied to specific enemies at battle start.")]
    public List<StartStatus> StartStatuses = new();

    [Tooltip("How many enemy turns the last enemy gets before fleeing (0 = no limit).")]
    public int TurnLimit = 0;

    [Tooltip("Reward granted when the last enemy is defeated.")]
    public EventOutcome WinReward;

    [Tooltip("Whether player statuses are ticked on the very first PlayerTurnState. " +
             "Set false to avoid ticking buffs/debuffs before the player acts.")]
    public bool TickStatusesOnFirstTurn = false;

    public override EventBehaviour AttachBehaviour(GameObject host)
    {
        BattleEvent b = host.AddComponent<BattleEvent>();
        b.Data = this;
        return b;
    }
}
