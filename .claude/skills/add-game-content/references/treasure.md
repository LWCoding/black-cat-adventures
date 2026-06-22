# Adding a new treasure

Unlike enemies, a treasure's effect is **custom C#**, not just data — `Treasure` ([Assets/Scripts/Battle/Treasures/Treasure.cs](../../../../Assets/Scripts/Battle/Treasures/Treasure.cs)) is abstract and every concrete treasure overrides `ActivateTreasure()`. There's no per-treasure prefab — `TreasureItem` instantiates straight from the ScriptableObject at battle start, so once the script + asset exist, you're done (no scene wiring needed, unlike enemies).

## 1. Find the closest existing pattern

Read 2-3 of the existing treasures under `Assets/Scripts/Battle/Treasures/` and match the user's described effect to the closest hook:

| Effect shape | Pattern to copy | Example |
|---|---|---|
| Flat damage bonus, always active | `DamageCalculator.RegisterFlatModifier(key, amount)` called once in `ActivateTreasure()` | `LuckyCatPaw.cs` |
| Flat/scaled damage bonus that depends on the *current word* | Subscribe to `WordPreview.Instance.OnLetterTilesChanged`, recompute, then call `RegisterFlatModifier`/`RegisterScaledModifier` each time the word changes | `FoolsGold.cs`, `MagicSevenBall.cs`, `ProfanityTape.cs` |
| Something that isn't damage-related at all (board manipulation, healing, status effects, etc.) | No existing treasure does this yet — search the codebase for the relevant manager (e.g. `Tile`/`WordGenerator`/`HealthHandler`/`StatusHandler`) to find the right hook before writing anything. Don't force it through `DamageCalculator` if it isn't actually a damage effect. | — |

`DamageCalculator.RegisterFlatModifier(key, amount, addOntoPreexistingValue=false)` and `RegisterScaledModifier(key, amount, multiplyByPreexistingValue=false)` both take a unique string `key` — use a short lowercase key derived from the treasure name (e.g. `"luckycatpaw"`), matching the existing convention. Re-registering the same key each word-change overwrites the previous value, which is how the "reset to 0 if condition no longer holds" pattern in `ProfanityTape`/`MagicSevenBall` works — copy that shape if the effect is conditional.

## 2. Write the C# script

Path: `Assets/Scripts/Battle/Treasures/<TreasureName>.cs`

```csharp
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "<Treasure Name>", menuName = "Treasures/<Treasure Name>")]
public class <TreasureName> : Treasure
{

    public override void ActivateTreasure()
    {
        // effect goes here, following the closest matching pattern above
    }

}
```

Match the existing file header style (`using System.Collections; using System.Collections.Generic; using UnityEngine;`) even if unused, since every existing treasure script has it — consistency over strict minimalism here.

## 3. Write the script's `.meta`

This is a brand-new script, so it has no Unity-assigned GUID yet. Generate one and write the `.meta` by hand — see "Writing a `.meta` for a brand-new C# script" in [asset-mechanics.md](asset-mechanics.md). Save the generated GUID; you need the identical value in the `.asset` file next.

## 4. Resolve the icon sprite

Find the PNG under `Assets/Images/Treasure/` (or wherever the user points you — confirm it exists, don't guess) and read its `.png.meta` for the GUID, same `fileID: 21300000` rule as enemy sprites (see asset-mechanics.md).

## 5. Write the `Treasure` `.asset`

Path: `Assets/Resources/ScriptableObjects/Treasure/<Treasure Name>.asset`

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
  m_Script: {fileID: 11500000, guid: <the script GUID you generated in step 3>, type: 3}
  m_Name: <Treasure Name>
  m_EditorClassIdentifier: 
  TreasureName: <Treasure Name>
  TreasureDescription: <description>
  TreasureIcon: {fileID: 21300000, guid: <icon sprite guid>, type: 3}
  IsUnlockedByDefault: 0
```

`IsUnlockedByDefault` — ask the user; most treasures are found/unlocked during play (`0`), only set `1` if they explicitly want the player to start with it.

## 6. Write the `.asset`'s own `.meta`

Same `NativeFormatImporter` template as enemies — see asset-mechanics.md's "brand-new `.asset` file" section. Generate a fresh GUID (distinct from the script's GUID from step 3).

## 7. Wiring up how it's obtained (ask, don't assume)

A new `Treasure` asset existing on disk doesn't make it reachable in-game. Ask the user whether they want it:
- collectible in a level (check `TreasureCollectible.cs` for the pattern — this is scene-placement work, similar caveat as enemy scene wiring), or
- just available for now and wired up later, in which case the script + asset alone is enough to hand off.

Don't add scene placement automatically without asking — it's a different scope than "create the treasure."
