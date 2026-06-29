---
name: add-status
description: Scaffolds a new status effect for the Black Cat Adventures Unity project — creates the concrete StatusEffect C# class, the .asset file, and wires the new type into the StatusEffectType enum. Use whenever the user asks to add a new status, debuff, buff, or character condition (e.g. "add a Burning status", "add a Frozen effect", "add a buff that increases damage for 2 turns").
---

# Adding a new status effect

Status effects are **concrete `StatusEffect` subclasses** stored as ScriptableObject `.asset` files. Each one is identified at runtime by the `StatusEffectType` enum rather than by name strings. There is no per-status prefab — a shared `Status.prefab` is instantiated for every effect and updated with the status's sprite/text by `StatusObject`.

## 0. Gather requirements first

Before writing anything, get:
- **Name** (also the C# class name, e.g. `Burning`)
- **Description** shown in the tooltip (`%d` is replaced with the current amplifier)
- **Mechanical effect**: what happens on `ApplyEffect` (on infliction) and `UpdateEffect` (each turn tick)?
- **Duration/amplifier semantics**: what does the amplifier represent — turns? damage per tick? both?
- **Icon sprite**: which PNG under `Assets/Images/Statuses/` to use; if none exists yet, use the Poison placeholder (`guid 56611bd15b94732499231d9b57258700`) and place the asset in `NeedsArt/`

## 1. Add the new value to `StatusEffectType`

In [Assets/Scripts/Battle/StatusEffects/StatusEffect.cs](../../../../Assets/Scripts/Battle/StatusEffects/StatusEffect.cs), append the new name at the end of the `StatusEffectType` enum (never reorder existing values — their int representations are stored in every `.asset`):

```csharp
public enum StatusEffectType
{
    Poison = 0,
    Regeneration = 1,
    Bruised = 2,
    Airborne = 3,
    Waterlogged = 4,
    Shrink = 5,
    Stunned = 6,
    // add new entry here with the next int
    Burning = 7,
}
```

## 2. Write the C# class

Path: `Assets/Scripts/Battle/StatusEffects/<StatusName>.cs`

```csharp
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "<Status Name>", menuName = "Status Effects/<Status Name>")]
public class <StatusName> : StatusEffect
{

    public override void ApplyEffect(CharacterHandler handler, int amplifier)
    {
        CurrAmplifier = amplifier;
        // any on-apply side effects (e.g. subscribe to events, register modifiers)
    }

    public override bool UpdateEffect(CharacterHandler handler)
    {
        CurrAmplifier--;
        // any per-turn side effects (e.g. damage, heal, buff)
        return CurrAmplifier <= 0;
    }

}
```

Key patterns to copy from existing effects:

| Pattern | Example |
|---|---|
| Deal damage each tick | `handler.HealthHandler.TakeDamage(CurrAmplifier);` then decrement (`Poison.cs`) |
| Heal each tick | `handler.HealthHandler.Heal(...)` (`Regeneration.cs`) |
| Modify damage while word is being built | `DamageCalculator.RegisterScaledModifier(key, value)` + subscribe to `WordPreview.Instance.OnLetterTilesChanged` (`Airborne.cs`, `Shrink.cs`) |
| Skip the caster's turn | handled externally by `PlayerTurnState`/`EnemyTurnState` checking `HasStatus(StatusEffectType.Stunned)` — see `Stunned.cs` for the minimal pattern |
| Grace turn (no effect on first tick) | set `private bool _justApplied = true;` and skip logic when true (`Poison.cs`) |

`ApplyEffect` is called once on infliction. `UpdateEffect` is called once per turn at the start of the afflicted character's turn (via `StatusHandler.RenderStatusEffectEffects()`). Return `true` to remove the effect.

If `ApplyEffect` subscribes to events or registers modifiers, **always unsubscribe/reset them** when `UpdateEffect` returns `true`.

## 3. Write the `.asset`

Path: `Assets/Resources/ScriptableObjects/Statuses/<Status Name>.asset`  
(Use `NeedsArt/` subfolder if using the Poison placeholder icon.)

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
  m_Script: {fileID: 11500000, guid: <script GUID — see below>, type: 3}
  m_Name: <Status Name>
  m_EditorClassIdentifier: 
  Type: <int value from enum above>
  Name: <Display Name>
  Description: <tooltip text, use %d for amplifier>
  Icon: {fileID: 21300000, guid: <icon sprite GUID>, type: 3}
  CurrAmplifier: 0
```

**Script GUID**: Unity generates the GUID for the `.cs` file on first import. Do not hand-write the `.meta` file — let Unity generate it. The GUID won't be known until Unity imports the file; write the `.asset` with a placeholder and then in the Unity Inspector drag the correct script onto the Script field after import to fix the reference.

**Icon GUID**: read `Assets/Images/Statuses/<name>.png.meta` for the `guid:` value, then use `{fileID: 21300000, guid: <that guid>, type: 3}`. The `fileID: 21300000` is Unity's fixed sub-asset ID for single-sprite textures.

**Poison placeholder GUID** (for NeedsArt items): `56611bd15b94732499231d9b57258700`

## 4. How status effects are applied in-game

There are three ways to inflict a status:

**A. Via treasure** — in the treasure's `ActivateTreasure()`, load the asset and call:
```csharp
StatusEffect myStatus = Resources.LoadAll<StatusEffect>("ScriptableObjects/Statuses")
    .First(s => s.Type == StatusEffectType.MyStatus);
// then on a hit event:
BattleManager.Instance.CurrEnemyHandler.StatusHandler.GainStatusEffect(myStatus, amplifier);
// or on the player:
BattleManager.Instance.PlayerHandler.StatusHandler.GainStatusEffect(myStatus, amplifier);
```
`Resources.LoadAll` recurses into `NeedsArt/` automatically.

**B. Via enemy attack** — set `InflictedStatuses` on an `EnemyAttack` in the enemy's `.asset`:
```yaml
InflictedStatuses:
- Target: 0   # 0 = PLAYER, 1 = ENEMY
  Status: {fileID: 11400000, guid: <the status .asset GUID>, type: 2}
  Amplifier: 2
```
`BattleManager.RenderAttackAgainstPlayer` applies these automatically.

**C. Via letter tile** — in a `TileType.ActivateTileEffects()` override, call `GainStatusEffect` directly (see `PoisonTile.cs`).

## 5. Checking for a status on a character

Use the type-safe enum overload — **do not pass strings**:
```csharp
handler.StatusHandler.HasStatus(StatusEffectType.MyStatus)
```

## 6. Turn-skip statuses (special case)

If the new status should **skip the affected character's turn**, add the check in the relevant turn state(s) in `Assets/Scripts/Battle/StateMachine/`:

```csharp
// In EnemyTurnState.OnEnterState — before OnEnemyAttack:
bool skipped = enemy.StatusHandler.HasStatus(StatusEffectType.MyStatus);
enemy.StatusHandler.RenderStatusEffectEffects();
if (skipped) { BattleManager.Instance.SetState(new PlayerTurnState()); return; }

// In PlayerTurnState.OnEnterState — before input:
bool skipped = sh.HasStatus(StatusEffectType.MyStatus);
sh.RenderStatusEffectEffects();
if (skipped) { BattleManager.Instance.SetState(new EnemyTurnState()); }
```

The stun check fires **before** `RenderStatusEffectEffects` so the amplifier is still > 0 when the skip triggers; the tick then decrements it normally.

## 7. Verification (in Unity)

After reimport:
1. Inspector on the new `.asset` shows no "Missing Script" warning; all fields populated.
2. `Type` field shows the correct enum value in the Inspector dropdown.
3. Play mode: the status icon appears on the correct character when inflicted; it ticks down each turn and disappears when expired.
4. `HasStatus(StatusEffectType.MyStatus)` returns the expected value in any conditional code.
