using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public static class GameManager
{

    private static List<Treasure> _allTreasures = new();
    private static List<Treasure> _equippedTreasures = new();
    public static List<Treasure> EquippedTreasures
    {
        get
        {
            // If we don't have any treasures, give the player the default treasure
            if (_equippedTreasures.Count == 0)
            {
                _allTreasures = Resources.LoadAll<Treasure>("ScriptableObjects/Treasure").ToList();
                List<Treasure> defaultTreasures = _allTreasures.FindAll((t) => t.IsUnlockedByDefault);
                _equippedTreasures = defaultTreasures;
            }
            // If we somehow have duplicate treasures, remove those
            _equippedTreasures = _equippedTreasures.Distinct().ToList();
            return _equippedTreasures;
        }
        set => _equippedTreasures = value;
    }

    public static List<string> LevelsCompleted = new();

    public static string CurrLevelSceneName = "";

}
