using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "Regeneration", menuName = "Status Effects/Regeneration")]
public class Regeneration : StatusEffect
{

    public override void ApplyEffect(CharacterHandler handler, int amplifier)
    {
        CurrAmplifier = amplifier;
        return;
    }

    public override bool UpdateEffect(CharacterHandler handler)
    {
        handler.HealthHandler.HealHealth(CurrAmplifier);
        CurrAmplifier--;
        return CurrAmplifier == 0;
    }

}