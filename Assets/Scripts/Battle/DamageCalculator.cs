using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class DamageCalculator
{

    private readonly static Dictionary<string, int> _flatModifiers = new();  // Modifiers that add/subtract
    private readonly static Dictionary<string, float> _scaleModifiers = new();  // Modifiers that mult/divide
    private static string _currLevelSceneName;

    /// <summary>
    /// Register a new flat modifier onto the damage value.
    /// If the level has changed since the last modifier has been
    /// added, resets both data containers and starts anew.
    /// </summary>
    public static void RegisterFlatModifier(string key, int mod, bool addOntoPreexistingValue = false)
    {
        if (_currLevelSceneName != GameManager.GameData.RecentLevelCompleted)
        {
            ResetModifiers();
        }
        _currLevelSceneName = GameManager.GameData.RecentLevelCompleted;
        if (!_flatModifiers.ContainsKey(key))
        {
            _flatModifiers[key] = 0;
        }
        _flatModifiers[key] = addOntoPreexistingValue ? _flatModifiers[key] + mod : mod;
    }

    /// <summary>
    /// Register a new multiplicative modifier onto the damage value.
    /// If the level has changed since the last modifier has been
    /// added, resets both data containers and starts anew.
    /// </summary>
    public static void RegisterScaledModifier(string key, float mod, bool multiplyByPreexistingValue = false)
    {
        if (_currLevelSceneName != GameManager.GameData.RecentLevelCompleted)
        {
            ResetModifiers();
        }
        _currLevelSceneName = GameManager.GameData.RecentLevelCompleted;
        if (!_scaleModifiers.ContainsKey(key))
        {
            _scaleModifiers[key] = 0;
        }
        _scaleModifiers[key] = multiplyByPreexistingValue ? _scaleModifiers[key] * mod : mod;
    }

    /// <summary>
    /// Clears both data containers.
    /// </summary>
    public static void ResetModifiers()
    {
        _flatModifiers.Clear();
        _scaleModifiers.Clear();
    }

    /// <summary>
    /// Given a list of tiles, calculates the number of damage it
    /// would deal in total. Applies any damage buffs/debuffs
    /// after the fact.
    /// 
    /// Optionally, supply `withModifiers = false` to calculate the
    /// raw damage without any modifiers.
    /// </summary>
    public static int CalculateDamage(List<Tile> tiles, bool withModifiers = true)
    {
        float total = 0;
        foreach (Tile tile in tiles)
        {
            switch (tile.DamageType)
            {
                case TileDamage.LOW:
                    total += 1;
                    break;
                case TileDamage.MEDIUM:
                    total += 1.4f;
                    break;
                case TileDamage.HIGH:
                    total += 1.8f;
                    break;
            }
        }
        int damage = (int)Mathf.Round(Mathf.Exp(total * 0.4f));
        // Calculate any buffs/debuffs dealt by external sources
        if (withModifiers)
        {
            foreach (int flatMod in _flatModifiers.Values)
            {
                damage += flatMod;
            }
            foreach (float scalarMod in _scaleModifiers.Values)
            {
                damage = (int)(damage * scalarMod);
            }
        }
        // Return final calculated damage
        return damage;
    }

    /// <summary>
    /// Given a list of tiles, calculates the damage added/subtracted
    /// due to modifiers.
    /// </summary>
    public static int CalculateDamageFromModifiers(List<Tile> tiles)
    {
        return CalculateDamage(tiles) - CalculateDamage(tiles, false);
    }

    /*
     * // If cat paw, add one damage
            if (TreasureSection.Instance.HasTreasure(typeof(LuckyCatPaw)))
            {
                damage += 1;
            }
            // If profane with right treasure, add one for each letter
            if (TreasureSection.Instance.HasTreasure(typeof(ProfanityTape)) && WordGenerator.Instance.IsProfaneWord(word))
            {
                damage += word.Length;
            }
            // If seven letters with right treasure, add seven damage
            if (TreasureSection.Instance.HasTreasure(typeof(MagicSevenBall)) && word.Length == 7)
            {
                damage += 7;
            }
     */
}
