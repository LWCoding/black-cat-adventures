using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "Stunned", menuName = "Status Effects/Stunned")]
public class Stunned : StatusEffect
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
