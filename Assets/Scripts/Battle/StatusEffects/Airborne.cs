using DG.Tweening;
using UnityEngine;

[CreateAssetMenu(fileName = "Airborne", menuName = "Status Effects/Airborne")]
public class Airborne : StatusEffect
{

    private CharacterHandler _handler;
    private Sequence _spriteToggle;

    public override void ApplyEffect(CharacterHandler handler, int amplifier)
    {
        CurrAmplifier = amplifier;
        if (handler is not EnemyHandler enemy) { return; }
        _handler = handler;
        WordPreview.Instance.OnLetterTilesChanged += AirborneEffect;
        AirborneEffect();
        enemy.SetHovering(true);
        enemy.HealthHandler.OnDeath += Land;
        StartSpriteToggle(enemy);
    }

    public override bool UpdateEffect(CharacterHandler handler)
    {
        CurrAmplifier--;
        if (CurrAmplifier == 0)
        {
            DamageCalculator.RegisterScaledModifier("airborne", 1f);
            Land();
        }
        return CurrAmplifier == 0;
    }

    /// <summary>
    /// Indefinitely flips between the idle and (optional) airborne sprite every 0.75s.
    /// No-op if the character has no AirborneSprite assigned.
    /// </summary>
    private void StartSpriteToggle(EnemyHandler enemy)
    {
        Sprite airborne = enemy.CharData.AirborneSprite.Sprite;
        if (airborne == null) { return; }
        Sprite idle = enemy.CharData.AliveSprite.Sprite;
        bool showingAirborne = false;
        _spriteToggle = DOTween.Sequence()
            .AppendInterval(0.75f)
            .AppendCallback(() =>
            {
                showingAirborne = !showingAirborne;
                enemy.SetSpriteImage(showingAirborne ? airborne : idle);
            })
            .SetLoops(-1);
    }

    /// <summary>
    /// Single cleanup path: runs when Airborne expires (amplifier hits zero) or the enemy dies.
    /// Stops the toggle loop, unsubscribes, lands, and returns to the idle sprite (unless dead).
    /// Idempotent, so it is safe if both triggers fire.
    /// </summary>
    private void Land()
    {
        if (_handler is not EnemyHandler enemy) { return; }
        _spriteToggle?.Kill();
        _spriteToggle = null;
        _handler = null;
        WordPreview.Instance.OnLetterTilesChanged -= AirborneEffect;
        enemy.HealthHandler.OnDeath -= Land;
        enemy.SetHovering(false);
        if (!enemy.HealthHandler.IsDead())
        {
            enemy.SetSpriteImage(enemy.CharData.AliveSprite.Sprite);
        }
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
