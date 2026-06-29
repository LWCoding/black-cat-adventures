using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum StatusEffectType
{
    Poison = 0,
    Regeneration = 1,
    Bruised = 2,
    Airborne = 3,
    Waterlogged = 4,
    Shrink = 5,
    Stunned = 6,
}

[System.Serializable]
public struct AttackStatus
{
    public Faction Target;
    public StatusEffect Status;
    public int Amplifier;
}

public abstract class StatusEffect : ScriptableObject
{

    [Header("Status Properties")]
    public StatusEffectType Type;
    public string Name;
    public string Description;
    public Sprite Icon;
    [HideInInspector] public int CurrAmplifier;

    /// <summary>
    /// Given a CharacterHandler, inflicts effects onto it (based on the
    /// nature of the status effect) after immediately applied.
    /// </summary>
    public abstract void ApplyEffect(CharacterHandler handler, int amplifier);

    /// <summary>
    /// Given a CharacterHandler, inflicts effects onto it (based on the
    /// nature of the status effect) over one turn.
    /// </summary>
    /// <returns>True if the effect should be removed, or else False.</returns>
    public abstract bool UpdateEffect(CharacterHandler handler);

}
