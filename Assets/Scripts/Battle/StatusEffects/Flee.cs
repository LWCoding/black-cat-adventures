using UnityEngine;

/// <summary>
/// Status applied to the Locked Treasure Box. Provides a visual turn countdown;
/// the box displays how many turns remain before it escapes. The actual flee
/// logic is driven by LockedTreasureEvent tracking EnemyTurnState transitions.
/// </summary>
[CreateAssetMenu(fileName = "Flee", menuName = "Status Effects/Flee")]
public class Flee : StatusEffect
{
    public override void ApplyEffect(CharacterHandler handler, int amplifier)
    {
        CurrAmplifier = amplifier;
    }

    public override bool UpdateEffect(CharacterHandler handler)
    {
        CurrAmplifier--;
        return CurrAmplifier <= 0;
    }
}
