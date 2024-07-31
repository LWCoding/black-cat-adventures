using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "Bruised", menuName = "Status Effects/Bruised")]
public class Bruised : StatusEffect
{

    public override void ApplyEffect(CharacterHandler handler, int amplifier)
    {
        CurrAmplifier = amplifier;
        return;
    }

    public override bool UpdateEffect(CharacterHandler handler)
    {
        handler.HealthHandler.TakeDamage(CurrAmplifier);
        CurrAmplifier--;
        return CurrAmplifier == 0;
    }

}
