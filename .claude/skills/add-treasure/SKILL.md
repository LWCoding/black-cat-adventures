---
name: add-treasure
description: Scaffolds a new treasure for the Black Cat Adventures Unity project — writes the concrete Treasure C# subclass and the .asset file. Use whenever the user asks to add a new treasure, item, collectible, relic, or battle ability/effect — even if they just describe the effect in plain language without using the word "skill" or "ScriptableObject". Always use this instead of hand-writing Unity .asset YAML from scratch.
---

# Adding a new treasure

Unlike enemies, a treasure's effect is **custom C#**, not just data — `Treasure` (`Assets/Scripts/Battle/Treasures/Treasure.cs`) is abstract and every concrete treasure overrides `ActivateTreasure()`. There is no per-treasure prefab — `TreasureItem` instantiates straight from the ScriptableObject at battle start, so once the script + asset exist, the treasure is fully functional with no scene wiring required.

## 0. Gather requirements first

Before writing anything, get from the user:
- **Name** (also the C# class name with no spaces, e.g. `LuckyCatPaw`)
- **Description** shown in the tooltip
- **The effect**, in plain language — what does it actually do mechanically?
- **Rarity** — Starter=0, Common=1, Rare=2, SuperRare=3, Legendary=4. Ask if unsure; most new treasures are Common (1) or Rare (2). Starter (0) is reserved for the treasure granted at game start.
- **Icon sprite** — which PNG under `Assets/Images/Treasure/` to use. Confirm it exists (`Glob Assets/Images/**/*<keyword>*`). If none exists yet, use the QuestionMark placeholder (GUID `d2feab8db485d734b91b7ab6b1ece18e`) and place the asset in the `NeedsArt/` subfolder.

## 1. Find the closest existing pattern

Read 2–3 existing treasures under `Assets/Scripts/Battle/Treasures/` and match the described effect to the closest hook:

| Effect shape | Pattern to copy | Example |
|---|---|---|
| Flat damage bonus, always active | `DamageCalculator.RegisterFlatModifier(key, amount)` once in `ActivateTreasure()` | `LuckyCatPaw.cs` |
| Damage bonus conditional on the current word | Subscribe to `WordPreview.Instance.OnLetterTilesChanged`, recompute, call `RegisterFlatModifier`/`RegisterScaledModifier` | `FoolsGold.cs`, `MagicSevenBall.cs`, `ProfanityTape.cs` |
| Board manipulation | `WordGrid.Instance.*` methods; see `FullBreakfast.cs` for the `EnsureLettersPresent` pattern | `FullBreakfast.cs` |
| Apply a status effect | Load the asset via `Resources.LoadAll<StatusEffect>("ScriptableObjects/Statuses").First(s => s.Type == StatusEffectType.MyStatus)`, then call `GainStatusEffect` | `Catnip.cs` |
| React to word submission | Subscribe to `SubmitButton.OnClickButton` | `Catnip.cs` |
| Modify shuffle behavior | Check/call `CreditCard.IsShuffleFree()` / see `ShuffleButton.cs` | `CreditCard.cs` |
| Anything else | Search the codebase for the relevant manager (`HealthHandler`, `StatusHandler`, `WordGenerator`, etc.) before writing anything — don't force it through `DamageCalculator` if it isn't a damage effect |

`DamageCalculator.RegisterFlatModifier(key, amount, addOntoPreexistingValue=false)` and `RegisterScaledModifier(key, amount, multiplyByPreexistingValue=false)` both take a unique string `key` — use a short lowercase key derived from the treasure name (e.g. `"luckycatpaw"`). Re-registering the same key each word-change **overwrites** the previous value, which is how the "reset to 0 when condition no longer holds" pattern works in `ProfanityTape`/`MagicSevenBall` — copy that shape for conditional effects.

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
        // effect goes here
    }

}
```

Keep the three `using` lines even if unused — all existing treasure scripts have them and consistency matters here.

## 3. Let Unity generate the `.meta`

Do **not** hand-write the `.meta` file for the new script. Unity auto-generates it on reimport. Just create the `.cs` file. The asset's `m_Script` GUID won't be known until after Unity imports the file; write the asset with a placeholder GUID and the user will reconcile it in the Inspector on first open.

## 4. Resolve the icon sprite

Read the PNG's `.meta` file for its GUID. The sprite sub-asset reference is always:

```yaml
{fileID: 21300000, guid: <texture guid>, type: 3}
```

`21300000` is Unity's fixed sub-asset ID for a single-sprite-mode texture. Do **not** invent a GUID — always read the `.meta` file. If using the QuestionMark placeholder: GUID `d2feab8db485d734b91b7ab6b1ece18e`.

## 5. Write the `Treasure` `.asset`

Path: `Assets/Resources/ScriptableObjects/Treasure/<Treasure Name>.asset`  
(Use `NeedsArt/` subfolder if using the QuestionMark placeholder icon.)

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
  m_Script: {fileID: 11500000, guid: <script GUID — see note in step 3>, type: 3}
  m_Name: <Treasure Name>
  m_EditorClassIdentifier: 
  TreasureName: <Treasure Name>
  TreasureDescription: <tooltip description>
  TreasureIcon: {fileID: 21300000, guid: <icon sprite guid>, type: 3}
  Rarity: <int>
```

**`Rarity` values**: Starter=0, Common=1, Rare=2, SuperRare=3, Legendary=4.

## 6. Let Unity generate the `.asset.meta` too

Same rule as the script — do **not** hand-write the `.asset.meta`. Unity generates it on reimport. `Resources.LoadAll<Treasure>()` recurses into `NeedsArt/` automatically, so subfolder placement doesn't affect discoverability.

## 7. Wiring up how it's obtained (ask, don't assume)

A new asset on disk doesn't make it reachable in-game by itself:
- It is automatically included in **post-battle reward rolls** (via `GameData.GetRandomUnownedTreasures`) as long as its `Rarity` is not `Starter` (0) — no extra wiring needed for this path.
- If it should also be collectible in a level scene (a `TreasureCollectible` pickup), that is scene-placement work — check `TreasureCollectible.cs` for the pattern and treat it as a separate task.

Ask the user which distribution path they want rather than assuming.

## 8. After reimport — verify in Unity

1. Open the project in Unity and let it reimport.
2. Inspect the new `.asset` — every field should be populated, not "None"/"Missing Script". If `m_Script` shows "Missing Script", open the Inspector, click the Script field, and assign the correct class.
3. Inspect the tooltip in-game: the treasure name should appear in the rarity color and the rarity label should show on the selection screen.
