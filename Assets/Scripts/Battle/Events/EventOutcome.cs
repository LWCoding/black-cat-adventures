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
    None                = 0,
    GrantRandomTreasures = 1,
    ChooseTreasure      = 2,
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
}
