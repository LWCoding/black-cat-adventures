# Adding a new enemy

An enemy is **pure data** — no new C# is needed for a standard enemy (stats + a list of attacks). You're writing two files: an `EnemyData` `.asset`, and a duplicated `Enemy.prefab`. Read [asset-mechanics.md](asset-mechanics.md) first for the GUID-lookup mechanics referenced below.

## 1. Resolve sprites

You need three: `AliveSprite`, `AttackSprite` (often the same image as Alive — check existing examples, many enemies reuse the idle sprite for both), and `DeadSprite`. Find the PNGs under `Assets/Images/Characters/` (use `Glob`/ask the user — don't guess filenames), then read each `.png.meta` to get its GUID (see asset-mechanics.md for the `fileID: 21300000` rule).

## 2. Write the `EnemyData` `.asset`

Path: `Assets/Resources/ScriptableObjects/Characters/<EnemyName>.asset`

Copy this structure exactly — it's the real format (based on `ArcFox.asset`), just with your values swapped in. The `m_Script` GUID is always `EnemyData`'s GUID (re-confirm it from `Assets/Scripts/Battle/Characters/EnemyData.cs.meta`, don't hardcode without checking):

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
    _attackDescription: <description, use %d as a placeholder for the damage number>
    Damage: <int>
    AnimType: 0
    InflictedStatuses: []
```

Notes on fields:
- **`Offset`** controls sprite positioning per-state and varies enemy to enemy — when unsure, `{x: 0, y: 0}` is a safe default; only fine-tune if the user cares about pixel positioning (which really needs the Editor to preview anyway).
- **`AnimType`** is the `AttackAnimation` enum: `0 = DEFAULT` (lunge-forward attack animation — use this unless told otherwise), `1 = NONE`, `2 = SAY_NOTHING` (enemy says "..." then attacks — used for silent/mysterious enemies).
- **`Attacks`** is a YAML list — repeat the block for each attack the user describes.
- **`InflictedStatuses`** stays `[]` unless the user wants a status effect (poison, etc.) applied. If so, look at how an existing attack does it by grepping the `Characters/*.asset` files for a non-empty `InflictedStatuses`, find the matching `Status` asset GUID under `Assets/Resources/ScriptableObjects/Statuses/`, and follow that exact pattern — `AttackStatus` has `Target` (`Faction`: `0 = PLAYER`, `1 = ENEMY`), `Status` (a `StatusEffect` asset reference), and `Amplifier` (int).
- **Icon sprites for attacks** live under `Assets/Images/Attack/` — same GUID lookup process.

## 3. Duplicate the prefab

Copy `Assets/Prefabs/Battle/Enemy.prefab` to `Assets/Prefabs/Battle/<EnemyName>.prefab` (a plain file copy — fileIDs only need to be unique *within* a single prefab file, so a straight duplicate is safe). Then edit exactly three things in the new copy:

1. The `GameObject` named `Enemy` (`m_Name: Enemy`) → rename to `<EnemyName>`.
2. The `EnemyHandler` component's `_charData` field — currently `{fileID: 11400000, guid: <some enemy's guid>, type: 2}` — change the `guid` to your new `EnemyData` asset's GUID. Since you just wrote that `.asset` yourself, you also need to hand-write its sibling `.meta` (see "Writing a `.meta` for a brand-new `.asset` file" in asset-mechanics.md) — generate a fresh GUID and use that same value here. The `type` stays `2`.
3. The `SpriteRenderer`'s `m_Sprite` field (the placeholder preview sprite shown in the editor before runtime sets it) → swap to your new enemy's `AliveSprite` GUID, same `{fileID: 21300000, ...}` pattern.

Don't touch anything else in the file — the dialogue box sub-prefab, status container, health text, etc. are shared structure that doesn't vary per enemy.

## 4. Hand off to the Editor (manual step — don't attempt to automate)

Tell the user:
- Open Unity and let it import the new `.asset` and `.prefab`.
- Drag the new prefab into the target scene (`Assets/Scenes/Level*.unity`).
- If it's not the last enemy in the encounter, set its `EnemyHandler._nextBattleObject` to the next enemy GameObject in the scene; if it *is* the last enemy, leave it `None`.
- Check the Inspector for any "Missing" reference — that means a GUID mismatch somewhere above.
