using DG.Tweening;
using UnityEngine;

[CreateAssetMenu(fileName = "Shrink", menuName = "Status Effects/Shrink")]
public class Shrink : StatusEffect
{

    private bool _justApplied = true;
    private SpriteScaleRoot _scaler;
    private float _shrinkValue;

    public override void ApplyEffect(CharacterHandler handler, int amplifier)
    {
        CurrAmplifier = amplifier;
        if (handler is not PlayerHandler) { return; }

        DamageCalculator.RegisterFlatModifier("shrink", -3);

        _scaler = handler.GetComponentInChildren<SpriteScaleRoot>();
        if (_scaler == null) { return; }

        // Animate the "shrink" modifier in three comical bouncing steps down to ~55%.
        // DOTween.To drives the value each frame so SpriteScaleRoot composites it with
        // any other active modifiers (e.g. squash/stretch) automatically.
        _shrinkValue = 1f;
        Sequence seq = DOTween.Sequence();
        seq.Append(DOTween.To(
            () => _shrinkValue,
            v => { _shrinkValue = v; _scaler.SetModifier("shrink", v); },
            0.78f, 0.29f).SetEase(Ease.OutQuad));
        seq.Append(DOTween.To(
            () => _shrinkValue,
            v => { _shrinkValue = v; _scaler.SetModifier("shrink", v); },
            0.65f, 0.29f).SetEase(Ease.OutBounce));
        seq.Append(DOTween.To(
            () => _shrinkValue,
            v => { _shrinkValue = v; _scaler.SetModifier("shrink", v); },
            0.55f, 0.42f).SetEase(Ease.OutElastic));
    }

    public override bool UpdateEffect(CharacterHandler handler)
    {
        if (_justApplied)
        {
            _justApplied = false;
            return CurrAmplifier == 0;
        }
        CurrAmplifier--;
        if (CurrAmplifier == 0)
        {
            DamageCalculator.RegisterFlatModifier("shrink", 0);
            if (_scaler != null)
            {
                DOTween.To(
                    () => _shrinkValue,
                    v => { _shrinkValue = v; _scaler.SetModifier("shrink", v); },
                    1f, 0.3f).SetEase(Ease.OutBack)
                    .OnComplete(() => _scaler.ClearModifier("shrink"));
            }
        }
        return CurrAmplifier == 0;
    }

}
