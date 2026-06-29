using UnityEngine;

/// <summary>
/// Central, lazily-loaded access point for the game's shared ScriptableObject
/// data. Mirrors the <see cref="GameManager"/> static-access pattern so any
/// script can read shared assets through one place instead of per-scene
/// serialized references or hard-coded Resources paths.
///
/// Every accessor loads on first use and caches for the rest of the session.
/// Lookups search all Resources folders by type, so the underlying assets can
/// be moved or renamed freely without breaking anything.
/// </summary>
public static class GameDatabase
{
    // ─── Singleton databases ─────────────────────────────────────────────────

    private static EventDatabase _events;
    public static EventDatabase Events => _events != null ? _events : _events = LoadSingle<EventDatabase>();

    private static EncounterDatabase _encounters;
    public static EncounterDatabase Encounters => _encounters != null ? _encounters : _encounters = LoadSingle<EncounterDatabase>();

    // ─── Map space singletons ────────────────────────────────────────────────

    private static BattleSpaceData _battleSpace;
    public static BattleSpaceData BattleSpace => _battleSpace != null ? _battleSpace : _battleSpace = LoadSingle<BattleSpaceData>();

    private static UnknownSpaceData _unknownSpace;
    public static UnknownSpaceData UnknownSpace => _unknownSpace != null ? _unknownSpace : _unknownSpace = LoadSingle<UnknownSpaceData>();

    private static MinibossSpaceData _minibossSpace;
    public static MinibossSpaceData MinibossSpace => _minibossSpace != null ? _minibossSpace : _minibossSpace = LoadSingle<MinibossSpaceData>();

    private static BossSpaceData _bossSpace;
    public static BossSpaceData BossSpace => _bossSpace != null ? _bossSpace : _bossSpace = LoadSingle<BossSpaceData>();

    private static EventSpaceData _eventSpace;
    public static EventSpaceData EventSpace => _eventSpace != null ? _eventSpace : _eventSpace = LoadSingle<EventSpaceData>();

    // ─── Category collections ────────────────────────────────────────────────

    private static Treasure[] _treasures;
    public static Treasure[] Treasures => _treasures ??= Resources.LoadAll<Treasure>("");

    private static StatusEffect[] _statuses;
    public static StatusEffect[] Statuses => _statuses ??= Resources.LoadAll<StatusEffect>("");

    private static TileType[] _tiles;
    public static TileType[] Tiles => _tiles ??= Resources.LoadAll<TileType>("");

    private static EnemyData[] _enemies;
    public static EnemyData[] Enemies => _enemies ??= Resources.LoadAll<EnemyData>("");

    private static SpaceData[] _spaces;
    public static SpaceData[] Spaces => _spaces ??= Resources.LoadAll<SpaceData>("");

    /// <summary>
    /// Loads the single asset of type <typeparamref name="T"/> from anywhere
    /// under a Resources folder. Logs if zero or more than one is found, since
    /// these types are expected to have exactly one instance in the project.
    /// </summary>
    private static T LoadSingle<T>() where T : UnityEngine.Object
    {
        T[] all = Resources.LoadAll<T>("");
        if (all.Length == 0)
        {
            Debug.LogError($"[GameDatabase] No {typeof(T).Name} asset found under any Resources folder.");
            return null;
        }
        if (all.Length > 1)
        {
            Debug.LogWarning($"[GameDatabase] Expected exactly one {typeof(T).Name} but found {all.Length}; using '{all[0].name}'.");
        }
        return all[0];
    }

    /// <summary>
    /// Clears every cache. Runs automatically when play mode starts so stale
    /// references never linger when "Enter Play Mode Options" disables the
    /// domain reload that would otherwise reset these static fields.
    /// </summary>
    [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.SubsystemRegistration)]
    private static void ResetCaches()
    {
        _events = null;
        _encounters = null;
        _battleSpace = null;
        _unknownSpace = null;
        _minibossSpace = null;
        _bossSpace = null;
        _eventSpace = null;
        _treasures = null;
        _statuses = null;
        _tiles = null;
        _enemies = null;
        _spaces = null;
    }
}
