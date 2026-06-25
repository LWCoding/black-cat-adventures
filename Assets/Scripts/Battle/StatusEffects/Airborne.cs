using UnityEngine;

[CreateAssetMenu(fileName = "Airborne", menuName = "Status Effects/Airborne")]
public class Airborne : StatusEffect
{

    public override void ApplyEffect(CharacterHandler handler, int amplifier)
    {
        CurrAmplifier = amplifier;
        if (handler is not EnemyHandler enemy) { return; }
        WordPreview.Instance.OnLetterTilesChanged += AirborneEffect;
        AirborneEffect();           // set modifier immediately so the current word is evaluated
        enemy.SetHovering(true);
    }

    public override bool UpdateEffect(CharacterHandler handler)
    {
        CurrAmplifier--;
        if (CurrAmplifier == 0)
        {
            // Reset modifier to 1 on expiry (avoids the lingering-modifier bug Waterlogged has)
            DamageCalculator.RegisterScaledModifier("airborne", 1f);
            if (handler is EnemyHandler enemy)
            {
                WordPreview.Instance.OnLetterTilesChanged -= AirborneEffect;
                enemy.SetHovering(false);
            }
        }
        return CurrAmplifier == 0;
    }

    private void AirborneEffect()
    {
        string word = (WordPreview.Instance.CurrentWord ?? "").ToUpperInvariant();
        // Words containing F, L, or Y cut through the air and deal full damage
        bool grounded = word.Contains('F') || word.Contains('L') || word.Contains('Y');
        DamageCalculator.RegisterScaledModifier("airborne", grounded ? 1f : 0f);
        if (!grounded && WordGenerator.Instance.IsValidWord(word))
        {
            BattleManager.Instance.RunNextFrame(() =>
            {
                if (WordGenerator.Instance.IsValidWord(WordPreview.Instance.CurrentWord))
                {
                    WordPreview.Instance.FeedbackText.enabled = true;
                    WordPreview.Instance.FeedbackText.text = "Airborne...";
                }
            });
        }
    }

}
