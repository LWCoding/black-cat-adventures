---
name: add-map-space
description: Adds a brand-new space type to the Black Cat Adventures map system — creates the SpaceData subclass C# script, the .asset, an optional new scene + Build Settings registration, and (if Unknown-eligible) wires the type into UnknownSpace.asset's Entries list with a configurable weight. Use whenever the user asks to add a new map node type, map event, or map space (e.g. "add a shop space", "add a rest site", "add a mystery event"). Always use this instead of hand-writing SpaceData YAML from scratch.
---

# Add Map Space

Black Cat Adventures represents map space types as `SpaceData` ScriptableObject subclasses referenced by GUID, not by name. A missing or wrong GUID produces a silent "missing script" reference in Unity with no compile error to catch it. Do **not** hand-write `.meta` files — Unity generates them on reimport. Read [references/space.md](references/space.md) before writing anything.

## 0. Gather requirements first

Before writing anything, ask the user for (or confirm from context):

1. **Space type name** — what is the new space called? (e.g. "Shop", "Rest Site", "Treasure Room"). This becomes the C# class name `<Name>SpaceData` and the `SpaceTypeId` string in the asset.

2. **Node sprite/visual** — which image file should appear on the map node when this space is revealed? Never invent a filename or assume one exists. `Glob` `Assets/Images/**/*<keyword>*` to find candidates, or ask the user which file to use. If the image isn't imported yet, stop and ask the user to import it first.

3. **Which scene this space loads** — ask one of:
   - Does it load an **existing** scene (e.g. `"Level"`, `"Map"`)? If so, what is the exact scene name?
   - Does it need a **new scene**? If so, read [references/scene-scaffold.md](references/scene-scaffold.md) — scaffold the scene files and register in Build Settings.

4. **What data the space passes to its scene** — the handoff is `GameManager.GameData.RecentLevelCompleted = payloadId` before `SceneManager.LoadScene(sceneName)`. What string does `payloadId` contain? (e.g. an encounter id, a shop id, a room id — or empty if the scene doesn't need a payload.) Does the scene need a new registry/lookup analogous to `EncounterRegistry`?

5. **ScriptableObject fields** — does the space data asset need extra fields beyond what `SpaceData` provides? (e.g. an item pool, a heal amount, a difficulty tag.) List them now so they go into the subclass.

6. **Unknown eligibility** — should an Unknown space be able to resolve into this new type?
   - If **yes**: what is the relative weight? (Weights are not percentages — the actual chance is `weight / sum(all weights in UnknownSpace.asset Entries)`.) For example, if Battle has weight 1 and you add Shop with weight 0.5, Battle occurs ~67% and Shop ~33% of Unknown resolutions.
   - If **no**: the type is never rolled by Unknown and this step is skipped.

## 1. Create the C# script

Path: `Assets/Scripts/Map/Spaces/<Name>SpaceData.cs`

Read [references/space.md](references/space.md) for the exact subclass template and required method signatures. The key contract:
- Class must be `public class <Name>SpaceData : SpaceData`
- Must have `[CreateAssetMenu(fileName = "New <Name> Space", menuName = "Spaces/<Name> Space")]`
- Must override `Resolve(System.Random rng)` and return a `ResolvedSpace` with `ResolvedTypeId = SpaceTypeId`, `SceneToLoad = SceneToLoad`, and `PayloadId` set to whatever the destination scene expects.

## 2. Create the ScriptableObject asset

Path: `Assets/Resources/ScriptableObjects/Spaces/<Name>Space.asset`

Do **not** hand-write the `.meta` files for the script or the asset — Unity generates them on reimport. Because the script's GUID won't be known until after import, write the asset with a placeholder `m_Script` GUID and fix it in the Inspector after reimport by dragging the correct script onto the Script field.

Read [references/space.md](references/space.md) for the exact YAML template. Key points:
- `m_Script` GUID = the GUID in the auto-generated `<Name>SpaceData.cs.meta` (read it after reimport; leave as placeholder until then).
- `SpaceTypeId` = the stable string id you chose in step 0 (e.g. `"Shop"`).
- `SceneToLoad` = the scene name from step 0.
- `NodeSprite` = `{fileID: 21300000, guid: <sprite guid>, type: 3}` using the sprite GUID read from the PNG's `.meta` file. If the user has not chosen a sprite yet, use `{fileID: 0}` and remind them to assign it in the Inspector.

The `fileID: 21300000` is Unity's fixed sub-asset ID for single-sprite-mode textures. Always read the actual PNG `.meta` for the GUID — never guess it.

## 3. (Conditional) Scaffold a new scene

If the space loads a scene that doesn't exist yet, read [references/scene-scaffold.md](references/scene-scaffold.md) and follow it completely. This includes:
- Writing the minimal `.unity` scene file (no hand-written `.meta` — Unity generates it).
- Adding the entry to `ProjectSettings/EditorBuildSettings.asset` after Unity generates the scene `.meta` and you can read its GUID.
- Creating a scene-reader MonoBehaviour that reads `GameManager.GameData.RecentLevelCompleted` and dispatches to the appropriate registry/loader.

## 4. (Conditional) Register in UnknownSpace.asset

If the space is Unknown-eligible (step 0, question 6 = yes), register it after Unity reimports and generates the asset's `.meta`:

**Preferred — Unity Inspector:** Select `UnknownSpace.asset`, click **+** on `Entries`, drag the new `.asset` into the `Space` slot, and set `Weight`.

**YAML fallback** (only after reimport): Read the newly generated `<Name>Space.asset.meta` for the GUID, then edit `UnknownSpace.asset` in place:

```yaml
- Space: {fileID: 11400000, guid: <asset guid from generated .meta>, type: 2}
  Weight: <weight from step 0>
```

Do **not** invent a GUID for `UnknownSpace.asset` itself — read the actual asset file and edit the `Entries` list in place.

## 5. Create a node prefab variant (manual Unity step)

Tell the user: in the Unity Editor, right-click `Assets/Prefabs/Map/Level.prefab` → **Create > Prefab Variant**. Name it `<Name>Space.prefab`. In the Inspector, set the `LevelHandler.AuthoredSpace` field to your new `<Name>Space.asset`. Optionally override the visual appearance (colors, additional child sprites) to distinguish this space type on the map.

## Always finish with a manual verification checklist

This skill writes files; it cannot run the Unity Editor. After writing everything, tell the user explicitly:

1. Open the project in Unity and let it reimport.
2. In the **Project** panel, select the new `<Name>Space.asset`. In the Inspector, confirm:
   - No "Missing Script" warning. If one appears, drag the correct `<Name>SpaceData` script onto the Script field.
   - `SpaceTypeId`, `DisplayName`, `SceneToLoad` all have correct values.
   - `NodeSprite` is assigned (not "None") — if it was left `{fileID: 0}`, assign it now.
   - Any extra fields (pool, registry reference, etc.) are populated.
3. Register the space in `UnknownSpace.asset` (if Unknown-eligible) via the Inspector — see step 4 above. Confirm the new entry appears in `Entries` with the correct `Space` reference and `Weight`.
4. If a new scene was created: open **File > Build Settings**, confirm the scene appears in the build list and is enabled.
5. In the **Map** scene, select a `LevelHandler` node and set its `AuthoredSpace` to `UnknownSpace.asset`. Enter Play mode — the Unknown node should resolve into your new type (visible in the Inspector via `GameData.ResolvedSpaces`) and the resolved sprite should appear on the map node.
6. Click the resolved node and press the battle button — confirm the correct scene loads and that `GameManager.GameData.RecentLevelCompleted` contains the expected `PayloadId` string.
7. Exit and re-enter Play mode — confirm the Unknown node shows the **same** resolved type (not re-rolled), proving persistence is working.
