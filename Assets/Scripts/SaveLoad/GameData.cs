using System.Collections.Generic;
using System.Linq;
using UnityEngine;

/// <summary>
/// Flat, JsonUtility-serializable record of per-node resolved space state.
/// Stored in GameData.ResolvedSpaces so Unknown spaces remain stable across reloads.
/// </summary>
[System.Serializable]
public class ResolvedSpaceEntry
{
    /// <summary>Stable id matching LevelHandler.SpaceNodeId.</summary>
    public string SpaceNodeId;
    /// <summary>SpaceTypeId of the concrete type this Unknown resolved into.</summary>
    public string ResolvedTypeId;
    /// <summary>Type-specific payload (e.g. encounter id for Battle spaces).</summary>
    public string PayloadId;
    /// <summary>Scene name to load when the player enters this space.</summary>
    public string SceneToLoad;
}

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
    public string RecentLevelCompleted = "";

    /// <summary>
    /// Stable node ids for each map node the player has beaten. Used for
    /// lock/unlock gating so progression is independent of which encounter
    /// a node happened to roll.
    /// </summary>
    public List<string> CompletedNodeIds = new();

    /// <summary>
    /// The SpaceNodeId of the most recently entered map node. Written by
    /// BattleButton before loading the battle scene; read by LevelsManager
    /// on return to restore the player to the correct position.
    /// </summary>
    public string RecentNodeEntered = "";

    /// <summary>
    /// Seed used to resolve Unknown spaces. Set once on the first map load and
    /// never changed thereafter, so all resolutions are reproducible.
    /// </summary>
    public int MapSeed;

    /// <summary>
    /// Persisted resolutions for each map node. Populated by LevelsManager on first
    /// load; looked up on subsequent loads to apply stable space appearances/payloads.
    /// </summary>
    public List<ResolvedSpaceEntry> ResolvedSpaces = new();

    public ResolvedSpaceEntry GetResolvedSpace(string spaceNodeId)
        => ResolvedSpaces.Find(e => e.SpaceNodeId == spaceNodeId);

    public void SetResolvedSpace(ResolvedSpaceEntry entry)
    {
        int idx = ResolvedSpaces.FindIndex(e => e.SpaceNodeId == entry.SpaceNodeId);
        if (idx >= 0)
        {
            ResolvedSpaces[idx] = entry;
        }
        else
        {
            ResolvedSpaces.Add(entry);
        }
    }

}
