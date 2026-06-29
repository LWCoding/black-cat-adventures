# Black Cat Adventures

## Naming conventions

### `*Manager` classes must be singletons

Any MonoBehaviour named `<Something>Manager` must derive from `Singleton<T>` ([Assets/Scripts/Utility/Singleton.cs](Assets/Scripts/Utility/Singleton.cs)) and expose itself via the inherited static `Instance` property, instead of hand-rolling singleton boilerplate.

```csharp
public class FooManager : Singleton<FooManager>
{
    protected override void Awake()
    {
        base.Awake();
        // ... rest of your setup
    }
}
```

Only override `Awake()` if you have extra setup to do; otherwise leave it out entirely and the base class handles it.

**Exception:** plain static classes with no MonoBehaviour/scene presence (e.g. `GameManager`, `SaveManager`) are not subject to this rule — they hold no per-instance state and aren't attached to a GameObject, so the singleton pattern doesn't apply to them.

## Data access

### Load shared ScriptableObject data through `GameDatabase`

`GameDatabase` ([Assets/Scripts/GameDatabase.cs](Assets/Scripts/GameDatabase.cs)) is the single, static access point for the game's shared ScriptableObject data. It lazily loads and caches each asset by type from anywhere under a `Resources` folder, so assets can be moved or renamed without breaking anything.

When you need shared game data, read it from `GameDatabase` instead of adding a hard-coded `Resources.Load`/`Resources.LoadAll` path or a per-scene serialized reference:

```csharp
// Preferred
EventData chosen = GameDatabase.Events.RollEvent(rng, GameManager.GameData.SeenEventIds);
Treasure[] all  = GameDatabase.Treasures;

// Avoid — hard-coded path, breaks if the asset moves
var db = Resources.Load<EncounterDatabase>("ScriptableObjects/Encounters/_EncounterDatabase");
// Avoid — per-scene [SerializeField] that must be re-wired in every scene
[SerializeField] private EventDatabase _eventDatabase;
```

It already exposes the singleton databases (`Events`, `Encounters`), the map space singletons (`BattleSpace`, `UnknownSpace`, `MinibossSpace`, `BossSpace`, `EventSpace`), and the category collections (`Treasures`, `Statuses`, `Tiles`, `Enemies`, `Spaces`).

**When you add a new kind of shared data, extend `GameDatabase` rather than reintroducing scattered paths or serialized refs.** Add a cached accessor there: use the `LoadSingle<T>()` helper for types that have exactly one instance, or a cached `Resources.LoadAll<T>("")` property for category collections, and remember to null the new cache field in `ResetCaches()`. Then have callers reference the new accessor.

This rule applies to ScriptableObject game data only. Loads that aren't shared SO data — e.g. parameterized prefab loaders or `TextAsset` files — stay where they are.

## Animations

### Prefer DOTween over manual coroutines

DOTween (`Assets/Plugins/Demigiant/DOTween/`) is installed project-wide. Use it for any positional, rotational, scale, color, or fade animation instead of hand-rolling `IEnumerator` / `Vector3.Lerp` coroutines.

```csharp
// Preferred
transform.DOMove(target, 0.25f).SetEase(Ease.InBack).OnComplete(() => Destroy(gameObject));

// Avoid
StartCoroutine(MoveCoroutine(target, 0.25f));
```

Use `SetDelay`, `SetEase`, `OnComplete`, and `DOSequence`/`Sequence.Join`/`Sequence.Append` for sequencing. Fall back to a coroutine only when the animation logic genuinely cannot be expressed as a tween (e.g. physics-driven or data-dependent per-frame decisions).

## Unity asset files

### Do not generate `.meta` files

Never write `.meta` files by hand. Unity generates them automatically on reimport, and a hand-written `.meta` can conflict with what the Editor would produce. Create only the actual asset or script file; let Unity handle the accompanying `.meta`.
