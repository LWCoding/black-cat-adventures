using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using Random = System.Random;

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
    public const int MaxEquippedTreasures = 5;

    [SerializeField] private List<Treasure> _unlockedTreasures = new();
    public List<Treasure> UnlockedTreasures
    {
        get
        {
            // If we don't have any treasures, give the player the default treasure
            if (_unlockedTreasures.Count == 0)
            {
                List<Treasure> allTreasures = GameDatabase.Treasures.ToList();
                List<Treasure> defaultTreasures = allTreasures.FindAll((t) => t is not None && t.Rarity == TreasureRarity.Starter);
                _unlockedTreasures = defaultTreasures;
            }
            // If we somehow have duplicate treasures, remove those
            _unlockedTreasures = _unlockedTreasures.Distinct().ToList();
            return _unlockedTreasures;
        }
        set => _unlockedTreasures = value;
    }
    public List<Treasure> EquippedTreasures
        => UnlockedTreasures.GetRange(0, System.Math.Min(MaxEquippedTreasures, UnlockedTreasures.Count));

    public List<string> LevelsCompleted = new();
    public string RecentLevelCompleted = "";

    /// <summary>
    /// Persisted flag: true once the player has finished or skipped the tutorial.
    /// Prevents the tutorial encounter and tooltips from replaying on continue.
    /// </summary>
    public bool HasTutorialCompleted;

    /// <summary>
    /// True once the player has seen the one-time tutorial explaining how
    /// activateable (ActiveTreasure) treasures work. Never shown again after.
    /// </summary>
    public bool HasSeenActiveTreasureTutorial;

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
    /// When true the next real battle (Level scene) begins with half the board
    /// scrambled to gold-etched (high-damage) letters. Cleared and saved the
    /// moment a battle consumes it, so a retry of a lost fight won't re-apply it.
    /// Set by the Genie Lamp event outcome.
    /// </summary>
    public bool NextBattleGoldEtched;

    /// <summary>
    /// Event ids that the player has fully experienced. Used by EventDatabase
    /// to prefer unseen events on future rolls.
    /// </summary>
    public List<string> SeenEventIds = new();

    /// <summary>
    /// ResolvedTypeId of the space the player is entering (e.g. "Battle", "Event").
    /// Written by BattleButton before scene load; read by LevelSpawner to decide
    /// whether to run normal encounter spawning or an event-mode branch.
    /// </summary>
    public string RecentResolvedTypeId = "";

    /// <summary>
    /// Persisted resolutions for each map node. Populated by LevelsManager on first
    /// load; looked up on subsequent loads to apply stable space appearances/payloads.
    /// </summary>
    public List<ResolvedSpaceEntry> ResolvedSpaces = new();

    /// <summary>
    /// True when the player owns at least one treasure that isn't the None placeholder.
    /// Used to gate events (e.g. Wishing Well) that require something to discard.
    /// </summary>
    public bool HasAnyTreasure => UnlockedTreasures.Any(t => t is not None);

    /// <summary>
    /// Removes and returns one random owned treasure (any rarity, including starters,
    /// but excluding the None placeholder and the optional <paramref name="exclude"/>
    /// treasure). Returns null if no eligible treasure exists.
    /// </summary>
    public Treasure DiscardRandomTreasure(Random rng, Treasure exclude = null)
    {
        List<Treasure> eligible = UnlockedTreasures.FindAll(t => t is not None && t != exclude);
        if (eligible.Count == 0) { return null; }
        Treasure chosen = eligible[rng.Next(eligible.Count)];
        _unlockedTreasures.Remove(chosen);
        return chosen;
    }

    private static readonly (TreasureRarity tier, int weight)[] _rollWeights =
    {
        (TreasureRarity.Common, 65), (TreasureRarity.Rare, 20),
        (TreasureRarity.SuperRare, 10), (TreasureRarity.Legendary, 5),
    };

    /// <summary>
    /// Returns up to <paramref name="count"/> treasures the player does not yet own,
    /// excluding None and Starter-tier treasures. Each slot is independently rolled
    /// by rarity weight (Common 65%, Rare 20%, Super Rare 10%, Legendary 5%),
    /// renormalizing over tiers that still have unowned entries to avoid empty draws.
    /// Returns fewer than count if the unowned pool is exhausted.
    /// </summary>
    public List<Treasure> GetRandomUnownedTreasures(int count)
    {
        List<Treasure> pool = GameDatabase.Treasures
            .Where(t => t is not None && t.Rarity != TreasureRarity.Starter && !UnlockedTreasures.Contains(t))
            .ToList();
        Random rng = new();
        List<Treasure> result = new();
        for (int n = 0; n < count && pool.Count > 0; n++)
        {
            int total = _rollWeights.Where(w => pool.Any(t => t.Rarity == w.tier)).Sum(w => w.weight);
            if (total == 0) { break; }
            int roll = rng.Next(total), acc = 0;
            TreasureRarity chosen = TreasureRarity.Common;
            foreach (var (tier, weight) in _rollWeights)
            {
                if (!pool.Any(t => t.Rarity == tier)) { continue; }
                acc += weight;
                if (roll < acc) { chosen = tier; break; }
            }
            List<Treasure> tierPool = pool.Where(t => t.Rarity == chosen).ToList();
            Treasure picked = tierPool[rng.Next(tierPool.Count)];
            result.Add(picked);
            pool.Remove(picked);
        }
        return result;
    }

    /// <summary>
    /// Migrates legacy saves that pre-date HasTutorialCompleted.
    /// If the player has any completed levels or map nodes beyond the auto-completed
    /// start node, they have already passed the tutorial.
    /// </summary>
    public void MigrateTutorialFlag()
    {
        if (HasTutorialCompleted) { return; }
        if (LevelsCompleted.Count > 0)
        {
            HasTutorialCompleted = true;
            return;
        }
        // CompletedNodeIds always contains at least the start node (Node_0_0),
        // so count > 1 means the player has beaten at least one real battle node.
        if (CompletedNodeIds.Count > 1)
        {
            HasTutorialCompleted = true;
            // Backfill LevelsCompleted from resolved battle-node payloads so
            // MinEncountersCompleted gating in EncounterDatabase stays accurate.
            foreach (ResolvedSpaceEntry entry in ResolvedSpaces)
            {
                if (!string.IsNullOrEmpty(entry.PayloadId)
                    && CompletedNodeIds.Contains(entry.SpaceNodeId))
                {
                    LevelsCompleted.Add(entry.PayloadId);
                }
            }
        }
    }

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
