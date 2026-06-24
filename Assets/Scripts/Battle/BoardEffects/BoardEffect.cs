using UnityEngine;

[System.Serializable]
public struct AttackBoardEffect
{
    public BoardEffect Effect;
    [Tooltip("Effect-specific magnitude (e.g. number of tiles to affect).")]
    public int Amplifier;
}

/// <summary>
/// Abstract base for enemy-driven, one-time mutations applied to the player's
/// board (WordGrid). Mirrors <see cref="StatusEffect"/> but is not persistent —
/// each subclass implements a single <see cref="Apply"/> call fired when the
/// owning attack resolves.
///
/// To add a new board effect: create a subclass here, a .asset in
/// Assets/Resources/ScriptableObjects/BoardEffects/, and reference it from
/// the attack's BoardEffects list in the enemy's EnemyData asset.
/// </summary>
public abstract class BoardEffect : ScriptableObject
{

    [Header("Board Effect Properties")]
    public string Name;
    [TextArea] public string Description;

    /// <summary>
    /// Applies this board effect. <paramref name="amplifier"/> is an
    /// effect-specific magnitude defined per-attack in the EnemyData asset
    /// (e.g. number of tiles to scramble, number of tiles to inflict a type on).
    /// </summary>
    public abstract void Apply(int amplifier);

}
