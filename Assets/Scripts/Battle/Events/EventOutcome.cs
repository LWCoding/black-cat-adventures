using System.Collections.Generic;

/// <summary>
/// The reward (or lack thereof) granted at the conclusion of an event option or
/// a won battle event. Serializable so it can be authored directly in .asset YAML
/// without additional ScriptableObject files.
/// Adding a new effect = one enum value + one case in EventOutcomes.GrantInBattle
/// or GrantInUI.
/// </summary>
public enum OutcomeType
{
    None                             = 0,
    GrantRandomTreasures             = 1,
    ChooseTreasure                   = 2,
    /// <summary>Wishing Well: discard a random owned treasure, gain one random new treasure.</summary>
    SwapRandomTreasure               = 3,
    /// <summary>Genie Lamp: gain one random treasure, then flag the next battle to start half-scrambled.</summary>
    GrantTreasureScrambleNextBattle  = 4,
}

[System.Serializable]
public struct EventOutcome
{
    /// <summary>Which reward to apply.</summary>
    public OutcomeType Type;
    /// <summary>For GrantRandomTreasures: how many treasures to grant directly.</summary>
    public int Count;
    /// <summary>For ChooseTreasure: how many options to show the player.</summary>
    public int ChooseFrom;
}

/// <summary>
/// Shared helpers for outcome execution that don't depend on scene context.
/// Scene-specific paths (TreasureChoiceScreen, fly animation, option buttons) live
/// in the behaviour classes that own the relevant scene.
/// </summary>
public static class EventOutcomes
{
    /// <summary>
    /// Draws <paramref name="count"/> random unowned treasures and adds them
    /// directly to GameData. Returns the list so callers can build result text.
    /// </summary>
    public static List<Treasure> GrantRandom(int count)
    {
        List<Treasure> granted = GameManager.GameData.GetRandomUnownedTreasures(count);
        foreach (Treasure t in granted)
        {
            GameManager.GameData.UnlockedTreasures.Add(t);
        }
        return granted;
    }

    /// <summary>
    /// Wishing Well outcome: grants one random unowned treasure (drawn first so it
    /// cannot be immediately discarded), then removes one random owned treasure.
    /// Returns (discarded, gained). Either may be null if the respective pool is empty.
    /// </summary>
    public static (Treasure discarded, Treasure gained) SwapRandomTreasure()
    {
        List<Treasure> gained = GrantRandom(1);
        Treasure gainedTreasure = gained.Count > 0 ? gained[0] : null;

        System.Random rng = new();
        Treasure discarded = GameManager.GameData.DiscardRandomTreasure(rng);
        return (discarded, gainedTreasure);
    }

    /// <summary>
    /// Genie Lamp outcome: grants one random treasure and flags the next real battle
    /// to begin with half its tiles scrambled to gold-etched (high-damage) letters.
    /// Returns the granted treasure, or null if the unowned pool is exhausted.
    /// </summary>
    public static Treasure GrantTreasureScrambleNextBattle()
    {
        List<Treasure> granted = GrantRandom(1);
        GameManager.GameData.NextBattleGoldEtched = true;
        return granted.Count > 0 ? granted[0] : null;
    }
}
