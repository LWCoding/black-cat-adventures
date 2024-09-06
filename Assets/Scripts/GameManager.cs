using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public static class GameManager
{

    private static List<Treasure> _allTreasures = new();
    private static List<Treasure> _unlockedTreasures = new();
    public static List<Treasure> UnlockedTreasures
    {
        get
        {
            // If we don't have any treasures, give the player the default treasure
            if (_unlockedTreasures.Count == 0)
            {
                _allTreasures = Resources.LoadAll<Treasure>("ScriptableObjects/Treasure").ToList();
                List<Treasure> defaultTreasures = _allTreasures.FindAll((t) => t.IsUnlockedByDefault);
                _unlockedTreasures = defaultTreasures;
            }
            // If we somehow have duplicate treasures, remove those
            _unlockedTreasures = _unlockedTreasures.Distinct().ToList();
            return _unlockedTreasures;
        }
        set => _unlockedTreasures = value;
    }
    public static List<Treasure> EquippedTreasures
    {
        get => _unlockedTreasures.GetRange(0, 3);
    }

    public static List<string> LevelsCompleted = new();

    public static string CurrLevelSceneName = "";

}
