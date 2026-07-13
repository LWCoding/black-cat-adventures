using UnityEngine;

/// <summary>
/// Base class for treasures the player actively triggers (via keybind or click)
/// during their turn, as opposed to passive treasures that only run at battle start.
/// This asset is shared and stateless: per-battle state (charges remaining, armed)
/// lives on the TreasureItem slot, never here.
/// </summary>
public abstract class ActiveTreasure : Treasure
{
    [Header("Active Treasure")]
    [Tooltip("How many times this treasure can be triggered per battle.")]
    public int MaxCharges = 1;

    [Tooltip("If true, triggering arms the treasure and waits for the player to click a target tile; the charge is spent when a valid tile is chosen.")]
    public bool RequiresTileTarget = true;

    /// <summary>
    /// Active treasures seal ActivateTreasure to route into RegisterPassiveEffects,
    /// keeping the battle-start hook available without exposing ActivateTreasure for override.
    /// </summary>
    public sealed override void ActivateTreasure() => RegisterPassiveEffects();

    /// <summary>
    /// Optional passive hooks that also run at battle start (e.g. subscribing to events).
    /// Default implementation does nothing.
    /// </summary>
    protected virtual void RegisterPassiveEffects() { }

    /// <summary>
    /// Extra runtime gating beyond charges and player-turn checks.
    /// Return false to prevent triggering (e.g. "only if a word is currently staged").
    /// Default: always triggerable while charges remain.
    /// </summary>
    public virtual bool CanTrigger() => true;

    /// <summary>
    /// Called the instant the player triggers this treasure (via key or click).
    /// For non-targeted treasures: perform the full effect here and return true.
    /// For targeted treasures: return false to stay armed and await OnTileTargeted.
    /// Default delegates to RequiresTileTarget (instant if false, await if true).
    /// </summary>
    public virtual bool OnTrigger() => !RequiresTileTarget;

    /// <summary>
    /// Called when the player clicks a grid or preview tile while this treasure is armed.
    /// Return true if the target was valid (charge will be spent and treasure disarmed).
    /// Return false to keep the treasure armed and wait for another click.
    /// </summary>
    public virtual bool OnTileTargeted(LetterTile tile) => true;
}
