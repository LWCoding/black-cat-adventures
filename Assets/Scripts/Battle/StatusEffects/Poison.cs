using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "Poison", menuName = "Status Effects/Poison")]
public class Poison : StatusEffect
{

    public override void ApplyEffect(CharacterHandler handler, int amplifier)
    {
        CurrAmplifier = amplifier;
        return;
    }

    public override bool UpdateEffect(CharacterHandler handler)
    {
        // Skip the first turn of effect
        if (JustApplied)
        {
            JustApplied = false;
            return CurrAmplifier == 0;
        }
        handler.HealthHandler.TakeDamage(CurrAmplifier);
        CurrAmplifier--;
        return CurrAmplifier == 0;
    }

}
