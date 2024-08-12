using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "Waterlogged", menuName = "Status Effects/Waterlogged")]
public class Waterlogged : StatusEffect
{

    private bool _justApplied = true;

    public override void ApplyEffect(CharacterHandler handler, int amplifier)
    {
        CurrAmplifier = amplifier;
        // Subscribe to effect when applied
        WordPreview.Instance.OnLetterTilesChanged += WaterlogEffect;
        return;
    }

    public override bool UpdateEffect(CharacterHandler handler)
    {
        // Skip the first turn of effect
        if (_justApplied)
        {
            _justApplied = false;
            return CurrAmplifier == 0;
        }
        CurrAmplifier--;
        // Unsubscribe from effect when it's time to remove
        if (CurrAmplifier == 0)
        {
            WordPreview.Instance.OnLetterTilesChanged -= WaterlogEffect;
        }
        return CurrAmplifier == 0;
    }

    private void WaterlogEffect()
    {
        if (WordGenerator.Instance.IsValidWord(WordPreview.Instance.CurrentWord) && WordPreview.Instance.CurrentTiles.Count < 5)
        {
            DamageCalculator.RegisterScaledModifier("waterlogged", 0);
        }
        else
        {
            DamageCalculator.RegisterScaledModifier("waterlogged", 1);
        }
    }

}
