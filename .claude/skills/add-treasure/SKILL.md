---
name: add-treasure
description: Scaffolds a new treasure for the Black Cat Adventures Unity project — writes the concrete Treasure C# subclass and the .asset file. Use whenever the user asks to add a new treasure, item, collectible, relic, or battle ability/effect — even if they just describe the effect in plain language without using the word "skill" or "ScriptableObject". Always use this instead of hand-writing Unity .asset YAML from scratch.
---

# Adding a new treasure

Unlike enemies, a treasure's effect is **custom C#**, not just data. There are two archetypes:

- **Passive** treasures extend `Treasure` (`Assets/Scripts/Battle/Treasures/Treasure.cs`) and override `ActivateTreasure()`. Their effect registers once at battle start and runs automatically thereafter.
- **Active** treasures extend `ActiveTreasure` (`Assets/Scripts/Battle/Treasures/ActiveTreasure.cs`) and are triggered by the player during their turn (keybinds 1-5 mapped to equipped slots, or by clicking the treasure). Triggering can resolve instantly or arm the treasure to wait for the player to click a target letter tile.

There is no per-treasure prefab — `TreasureItem` instantiates straight from the ScriptableObject at battle start, so once the script + asset exist, the treasure is fully functional with no scene wiring required. The keybind badge, "armed" highlight, charge tracking, and tile-targeting flow are all handled automatically by `TreasureItem`/`TreasureSection` for any `ActiveTreasure`.

## 0. Gather requirements first

Before writing anything, get from the user:
- **Name** (also the C# class name with no spaces, e.g. `LuckyCatPaw`)
- **Description** shown in the tooltip
- **Active or passive?** — Ask the user (or infer from the described effect) whether this treasure has an **active ability** the player triggers, or a **passive effect** that just runs on its own:
  - **Passive** — always-on or automatically-reacting effects (flat/conditional damage bonuses, board setup, status on submit, etc.). Extends `Treasure`. This is the default for most treasures.
  - **Active** — the player must press its keybind (1-5) or click it to use it. Extends `ActiveTreasure`. Signals include: "the player can activate it", "press to use", "once per battle/turn", "choose/target a tile", or any effect that shouldn't happen automatically.
  - If active, also gather:
    - **Charges** — how many times it can be used per battle (`MaxCharges`, default `1`).
    - **Needs a target tile?** — does using it require the player to then click a letter tile in the grid/preview (`RequiresTileTarget`)? If the effect is instant (no tile pick), set this to `false` and do the work in `OnTrigger()`.
- **The effect**, in plain language — what does it actually do mechanically?
- **Rarity** — Starter=0, Common=1, Rare=2, SuperRare=3, Legendary=4. Ask if unsure; most new treasures are Common (1) or Rare (2). Starter (0) is reserved for the treasure granted at game start.
- **Icon sprite** — which PNG under `Assets/Images/Treasure/` to use. Confirm it exists (`Glob Assets/Images/**/*<keyword>*`). If none exists yet, use the QuestionMark placeholder (GUID `d2feab8db485d734b91b7ab6b1ece18e`) and place the asset in the `NeedsArt/` subfolder.

## 1. Find the closest existing pattern

If the treasure is **active**, skip to [section 1a](#1a-active-treasures). Otherwise, for **passive** treasures, read 2–3 existing treasures under `Assets/Scripts/Battle/Treasures/` and match the described effect to the closest hook:

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

### 1a. Active treasures

Active treasures extend `ActiveTreasure` (not `Treasure`). You do **not** override `ActivateTreasure()` (it's sealed); instead override the hooks you need:

| Hook | When it runs | Return value |
|---|---|---|
| `bool OnTrigger()` | The instant the player presses the keybind or clicks the treasure. | `true` = effect resolved now, spend a charge immediately. `false` = stay "armed" and wait for the player to click a tile. Default returns `!RequiresTileTarget`. |
| `bool OnTileTargeted(LetterTile tile)` | The player clicks a grid/preview tile while the treasure is armed. | `true` = target valid, spend charge and disarm. `false` = keep waiting for another click. |
| `bool CanTrigger()` | Extra gating checked before arming (beyond charges + player turn). | `false` blocks triggering, e.g. "only when a word is staged". Default `true`. |
| `void RegisterPassiveEffects()` | Battle start (like a passive treasure's `ActivateTreasure`). | Optional passive hooks that coexist with the active ability. Default does nothing. |

`MaxCharges` (uses per battle) and `RequiresTileTarget` are inspector fields set on the `.asset`, not in code. Charges, the keybind badge, the armed highlight, and Escape-to-cancel are all handled automatically — the subclass only implements the effect.

Two common shapes:
- **Targeted** (default): leave `RequiresTileTarget: 1`, do the work in `OnTileTargeted`. Example — `Elixir.cs` turns the clicked tile into an "E":
  ```csharp
  public override bool OnTileTargeted(LetterTile tile)
  {
      if (tile == null || tile.Tile == null) { return false; }
      tile.Tile.Letters = "E";
      tile.InitializeTile(tile.Tile);
      return true;
  }
  ```
- **Instant** (no target): set `RequiresTileTarget: 0` in the asset, do the work in `OnTrigger()` and `return true`.

## 2. Write the C# script

Path: `Assets/Scripts/Battle/Treasures/<TreasureName>.cs`

**Passive treasure:**

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

**Active treasure:**

```csharp
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "<Treasure Name>", menuName = "Treasures/<Treasure Name>")]
public class <TreasureName> : ActiveTreasure
{

    // For a targeted treasure, implement the effect on the clicked tile:
    public override bool OnTileTargeted(LetterTile tile)
    {
        // effect goes here; return true when the target is valid
        return true;
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

**Active treasures** additionally need their two `ActiveTreasure` fields serialized (omitting them makes Unity read `0`/`false`, not the C# defaults):

```yaml
  MaxCharges: <int>          # uses per battle, e.g. 1
  RequiresTileTarget: <0|1>  # 1 = arm and wait for a tile click; 0 = instant
```

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
