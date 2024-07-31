using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public struct AttackStatus
{
    public Faction Target;
    public StatusEffect Status;
    public int Amplifier;
}

public abstract class StatusEffect : ScriptableObject
{

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
