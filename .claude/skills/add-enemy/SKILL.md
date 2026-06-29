---
name: add-enemy
description: Scaffolds a new enemy for the Black Cat Adventures Unity project — writes the EnemyData ScriptableObject .asset and a duplicated enemy prefab with the correct sprite and data references. Use whenever the user asks to add a new enemy, boss, monster, or foe, or describes a new enemy's health/attacks in plain language. Always use this instead of hand-writing Unity .asset YAML from scratch.
---

# Adding a new enemy

An enemy is **pure data** — no new C# is needed for a standard enemy. You're writing two files: an `EnemyData` `.asset` and a duplicated `Enemy.prefab`.

## 0. Gather requirements first

Before writing anything, get from the user (ask if not given):
- **Name**
- **Flavor text** (`EnemyDescription`)
- **Starting health**
- **Attacks**: for each attack — name, damage, description, any status effects inflicted, any `AvoidIf*` conditions
- **Which already-imported images to use.** Never invent a filename — `Glob` `Assets/Images/**/*<keyword>*` to find candidates, or ask. If the image isn't imported yet, stop and tell the user to import it first.

## 1. Resolve sprites

You need three: `AliveSprite`, `AttackSprite` (often the same image as Alive — check existing examples), and `DeadSprite`. For each PNG under `Assets/Images/Characters/`, read its `.png.meta` to get its GUID. The sprite sub-asset reference is always:

```yaml
{fileID: 21300000, guid: <texture guid>, type: 3}
```

`21300000` is Unity's fixed sub-asset ID for a single-sprite-mode texture. Do **not** invent a GUID — always read the `.meta` file.

## 2. Write the `EnemyData` `.asset`

Path: `Assets/Resources/ScriptableObjects/Characters/<EnemyName>.asset`

Base the structure on `ArcFox.asset`. The `m_Script` GUID is always `EnemyData`'s — read it fresh from `Assets/Scripts/Battle/Characters/EnemyData.cs.meta` (confirmed stable GUID `cf72fda75ee21d9469e20180c8adc802`, but always re-read to be safe):

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
  m_Script: {fileID: 11500000, guid: <EnemyData script guid>, type: 3}
  m_Name: <EnemyName>
  m_EditorClassIdentifier: 
  StartingHealth: <int>
  AliveSprite:
    Sprite: {fileID: 21300000, guid: <alive sprite guid>, type: 3}
    Offset: {x: 0, y: 0}
  AttackSprite:
    Sprite: {fileID: 21300000, guid: <attack sprite guid>, type: 3}
    Offset: {x: 0, y: 0}
  DeadSprite:
    Sprite: {fileID: 21300000, guid: <dead sprite guid>, type: 3}
    Offset: {x: 0, y: 0}
  SpriteScale: {x: 1, y: 1}
  EnemyDescription: <flavor text>
  Attacks:
  - IconSprite: {fileID: 21300000, guid: <attack icon sprite guid>, type: 3}
    AttackName: <name>
    _attackDescription: <description — use %d as a placeholder for the damage number>
    Damage: <int>
    AnimType: 0
    InflictedStatuses: []
    BoardEffects: []
    AvoidIfSelfHasStatus: {fileID: 0}
    AvoidIfTargetHasStatus: {fileID: 0}
    ProjectileSprite: {fileID: 0}
    ProjectileSpeed: 0
```

**Field notes:**

- **`Offset`** — controls sprite positioning per-state; `{x: 0, y: 0}` is a safe default when unsure.
- **`AnimType`** — `AttackAnimation` enum: `0 = DEFAULT` (lunge), `1 = NONE`, `2 = SAY_NOTHING` (enemy says "..." first), `3 = PROJECTILE`.
- **`Attacks`** — repeat the block for each attack.
- **`InflictedStatuses`** — stays `[]` unless the user wants a status applied. If so, look at an existing enemy asset that uses statuses (grep `Characters/*.asset` for a non-empty `InflictedStatuses`) and match the exact pattern. `AttackStatus` has `Target` (`Faction`: `0 = PLAYER`, `1 = ENEMY`), `Status` (a `StatusEffect` asset reference), and `Amplifier` (int).
- **Attack icon sprites** live under `Assets/Images/Attack/` — same GUID lookup process.
- **`AvoidIfSelfHasStatus` / `AvoidIfTargetHasStatus`** — set to a `StatusEffect` asset reference if the attack should be skipped when the enemy/player already has a certain status; otherwise `{fileID: 0}`.

## 3. Let Unity generate the `.meta`

Do **not** hand-write the `.asset.meta`. Unity auto-generates it on reimport.

## 4. Duplicate the prefab

Copy `Assets/Prefabs/Battle/Enemy.prefab` to `Assets/Prefabs/Battle/<EnemyName>.prefab` (a plain file copy — fileIDs only need to be unique within a single prefab). Then edit exactly three things in the new copy:

1. The `GameObject` named `Enemy` (`m_Name: Enemy`) → rename to `<EnemyName>`.
2. The `EnemyHandler` component's `_charData` field — currently `{fileID: 11400000, guid: <some enemy's guid>, type: 2}` — change the `guid` to your new `EnemyData` asset's GUID. Since Unity hasn't generated the asset's `.meta` yet, note that the GUID won't be known until after reimport. Write the prefab with a temporary value and remind the user to update `_charData` in the Inspector after reimport.
3. The `SpriteRenderer`'s `m_Sprite` field → swap to your new enemy's `AliveSprite` GUID: `{fileID: 21300000, guid: <alive guid>, type: 3}`.

Don't touch anything else — the dialogue box sub-prefab, status container, health text, etc. are shared structure that doesn't vary per enemy.

## 5. Edge case: bespoke enemy mechanics

If the enemy needs unique runtime behavior beyond "stats + attacks" (a boss gimmick, multi-phase fight, an attack that isn't flat damage), that requires a custom `EnemyHandler` subclass — not just data. Investigate `EnemyHandler` (`Assets/Scripts/Battle/Characters/EnemyHandler.cs`) and `CharacterHandler` first, explain the extra scope to the user, and treat it as a design task rather than pure scaffolding.

## 6. Hand off to the Editor (manual step — don't attempt to automate)

Tell the user:
- Open Unity and let it import the new `.asset` and `.prefab`.
- Drag the new prefab into the target scene (`Assets/Scenes/Level*.unity`).
- If it's not the last enemy in the encounter, set its `EnemyHandler._nextBattleObject` to the next enemy GameObject; if it *is* the last enemy, leave it `None`.
- Check the Inspector for any "Missing" reference — that means a GUID mismatch somewhere above.
