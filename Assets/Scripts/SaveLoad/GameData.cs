using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

[System.Serializable]
public class GameData
{

    [SerializeField] private List<Treasure> _unlockedTreasures = new();
    public List<Treasure> UnlockedTreasures
    {
        get
        {
            // If we don't have any treasures, give the player the default treasure
            if (_unlockedTreasures.Count == 0)
            {
                List<Treasure> allTreasures = Resources.LoadAll<Treasure>("ScriptableObjects/Treasure").ToList();
                List<Treasure> defaultTreasures = allTreasures.FindAll((t) => t.IsUnlockedByDefault);
                _unlockedTreasures = defaultTreasures;
            }
            // If we somehow have duplicate treasures, remove those
            _unlockedTreasures = _unlockedTreasures.Distinct().ToList();
            return _unlockedTreasures;
        }
        set => _unlockedTreasures = value;
    }
    public List<Treasure> EquippedTreasures
    {
        get => UnlockedTreasures.GetRange(0, 3);
    }

    public List<string> LevelsCompleted = new();
    public string RecentLevelCompleted = "";  // Name of scene

}
