# Adding a new SpaceData subclass

A map space type is a ScriptableObject that subclasses `SpaceData` (`Assets/Scripts/Map/Spaces/SpaceData.cs`). The single required contract is overriding `Resolve(System.Random rng)` to return a `ResolvedSpace` describing which scene to load and what payload to hand off. Do **not** hand-write `.meta` files — Unity generates them on reimport. For GUIDs that must be known before reimport (e.g. sprite references), always read the existing PNG `.meta` files to get the correct value.

## 1. C# subclass template

Path: `Assets/Scripts/Map/Spaces/<Name>SpaceData.cs`

```csharp
using UnityEngine;

[CreateAssetMenu(fileName = "New <Name> Space", menuName = "Spaces/<Name> Space")]
public class <Name>SpaceData : SpaceData
{

    // Add any extra fields your space type needs:
    // public EncounterPool Pool;
    // public int HealAmount;
    // etc.

    public override ResolvedSpace Resolve(System.Random rng)
    {
        // Compute your PayloadId here (e.g. a specific item id, a room id, or empty).
        string payloadId = ComputePayload(rng);

        return new ResolvedSpace
        {
            ResolvedTypeId = SpaceTypeId,   // matches this asset's SpaceTypeId field
            SceneToLoad = SceneToLoad,      // matches this asset's SceneToLoad field
            PayloadId = payloadId,
        };
    }

    private string ComputePayload(System.Random rng)
    {
        // Example: roll from a pool.
        // return Pool != null ? Pool.Roll(rng)?.SomeId ?? string.Empty : string.Empty;
        return string.Empty;
    }

}
```

Notes:
- `SpaceTypeId` and `SceneToLoad` come from the base `SpaceData` fields set in the asset inspector — don't hardcode strings in `Resolve`.
- For a weighted pool, follow the pattern in `EncounterPool.Roll(System.Random)` (`Assets/Scripts/Battle/Levels/EncounterPool.cs`).
- The `System.Random rng` passed in is seeded from `GameData.MapSeed` so results are stable across reloads. Do not create a new `System.Random` inside `Resolve` — always use the one passed in.

## 2. Asset YAML template

Path: `Assets/Resources/ScriptableObjects/Spaces/<Name>Space.asset`

```yaml
%YAML 1.1
%TAG !u! tag:unity3d.com,2011:
--- !u!114 &11400000
MonoBehaviour:
  m_ObjectHideFlags: 0
  m_CorrespondingSourceObject: {fileID: 0}
  m_PrefabInstance: {fileID: 0}
  m_PrefabAsset: {fileID: 0}
  m_GameObject: {fileID: 0}
  m_Enabled: 1
  m_EditorHideFlags: 0
  m_Script: {fileID: 11500000, guid: <script guid — read from generated .cs.meta after reimport; use placeholder until then>, type: 3}
  m_Name: <Name>Space
  m_EditorClassIdentifier: 
  SpaceTypeId: <stable type id, e.g. Shop>
  DisplayName: <display name, e.g. Shop>
  NodeSprite: {fileID: 21300000, guid: <sprite guid>, type: 3}
  SceneToLoad: <unity scene name, e.g. Shop>
  # Add extra fields below, matching the serialized names in your C# class:
  # Pool: {fileID: 11400000, guid: <pool asset guid>, type: 2}
  # HealAmount: 10
```

- `NodeSprite` uses `fileID: 21300000` for single-mode PNG sprites (see asset-mechanics.md). If no sprite is assigned yet, use `{fileID: 0}` and assign in Inspector.
- Extra reference fields use `{fileID: 11400000, guid: <asset guid>, type: 2}`.
- The `m_Script` GUID must match the auto-generated `<Name>SpaceData.cs.meta` — read that file after Unity imports the script. If registering immediately, leave a placeholder and fix via the Inspector after import.

## 3. UnknownSpace.asset — adding an entry

Read `Assets/Resources/ScriptableObjects/Spaces/UnknownSpace.asset` first to see the current entries. Append your new entry inside the `Entries:` list:

```yaml
  - Space: {fileID: 11400000, guid: <your new asset guid — read from generated .asset.meta after reimport>, type: 2}
    Weight: <float — relative, not a percentage>
```

**Weight semantics:** Unknown picks a random point in `[0, totalWeight)` where `totalWeight` is the sum of all entry weights. A weight of `1` alongside another weight of `1` means 50/50. A weight of `0.5` alongside `1` means ~33% for yours and ~67% for the existing. Never set a weight to `0` unless you want to temporarily disable the entry without removing it.

## 4. Resolving the PayloadId in the destination scene

The destination scene's bootstrap MonoBehaviour reads the payload like this:

```csharp
private void Awake()
{
    string payloadId = GameManager.GameData.RecentLevelCompleted;
    // Use payloadId to look up the right data from your registry:
    MyData data = MyRegistry.GetData(payloadId);
    // ... initialize the scene
}
```

For Battle spaces this is `EncounterRegistry.GetEncounter(payloadId)` in `LevelSpawner`. Copy that pattern for new registries.

If your space doesn't need a payload (e.g. a Rest site that always heals the same amount), return `PayloadId = string.Empty` from `Resolve` and just ignore `RecentLevelCompleted` in the destination scene's `Awake`.
