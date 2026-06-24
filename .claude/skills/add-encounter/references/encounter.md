# Writing the Encounter `.asset`

An `Encounter` is a `MonoBehaviour` ScriptableObject (`Assets/Scripts/Battle/Levels/Encounter.cs`) whose `Enemies` field is an ordered `List<EnemySpawn>`. The list order is the fight order: index 0 is the first enemy the player faces, the last entry is the final enemy before the auto-appended treasure chest.

## Field reference

`Encounter`:
- `EncounterId` (string) — stable id used by `EncounterDatabase.GetEncounter` and map handoff. Match the asset filename.
- `Enemies` (`List<EnemySpawn>`) — see below.

`EnemySpawn` (`Assets/Scripts/Battle/Levels/EnemySpawn.cs`):
- `EnemyData` — reference to the enemy's `EnemyData` `.asset`: `{fileID: 11400000, guid: <enemy guid>, type: 2}`.
- `DialogueToPlayOnMeet` (`List<DialogueInfo>`) — lines shown when the player reaches this enemy. May be empty (`DialogueToPlayOnMeet: []`).
- `ShouldStallBeforeTurn` (bool) — `0`/`1`. Default `0`.
- `TimeToNextObject` (float) — pacing delay. Default `1.3`.
- `SpawnOffset` (Vector2) — position nudge. Default `{x: 0, y: 0}`.

`DialogueInfo` (`Assets/Scripts/Battle/World UI/DialogueBoxHandler.cs`):
- `Text` (string) — the line.
- `Duration` (float) — seconds shown (existing encounters use `2`–`3`).
- `Speaker` (`Faction` enum) — **`0 = PLAYER`, `1 = ENEMY`**.
- `ShouldStallState` (bool) — `1` = battle pauses (`WaitState`) while the line shows; `0` = line plays without halting flow. Default `0`.

## `.asset` template

Copy this exactly (it's the real `Thieves.asset` format with values swapped in). Re-confirm the `m_Script` GUID from `Assets/Scripts/Battle/Levels/Encounter.cs.meta` (currently `0f09b01bdfcd89596d2250fbbae52cc1`):

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
  m_Script: {fileID: 11500000, guid: 0f09b01bdfcd89596d2250fbbae52cc1, type: 3}
  m_Name: <EncounterName>
  m_EditorClassIdentifier: 
  EncounterId: <EncounterId>
  Enemies:
  - EnemyData: {fileID: 11400000, guid: <enemy 1 EnemyData guid>, type: 2}
    DialogueToPlayOnMeet:
    - Text: <line said when reaching enemy 1>
      Duration: 2
      Speaker: 1
      ShouldStallState: 1
    - Text: <another line, optional>
      Duration: 2
      Speaker: 1
      ShouldStallState: 0
    ShouldStallBeforeTurn: 0
    TimeToNextObject: 1.3
    SpawnOffset: {x: 0, y: 0}
  - EnemyData: {fileID: 11400000, guid: <enemy 2 EnemyData guid>, type: 2}
    DialogueToPlayOnMeet: []
    ShouldStallBeforeTurn: 0
    TimeToNextObject: 1.3
    SpawnOffset: {x: 0, y: 0}
```

Repeat the `- EnemyData:` block for each enemy, in fight order. For an enemy with no dialogue, use `DialogueToPlayOnMeet: []` (as in enemy 2 above). For dialogue, repeat the `- Text:` block per line.

## `.asset.meta` template

Hand-write the sibling `.meta` with a fresh GUID (distinct from the script and every enemy):

```yaml
fileFormatVersion: 2
guid: <32 lowercase hex chars, no dashes — fresh>
NativeFormatImporter:
  externalObjects: {}
  mainObjectFileID: 11400000
  userData: 
  assetBundleName: 
  assetBundleVariant: 
```

This `guid` is what `_EncounterDatabase.asset` references the encounter by.

# Registering in `_EncounterDatabase.asset`

`EncounterDatabase` (`Assets/Scripts/Battle/Levels/EncounterDatabase.cs`) is the single source of truth: it does id lookup (`GetEncounter`) and weighted random selection (`Roll`, filtered by `MinEncountersCompleted`). The asset lives at `Assets/Resources/ScriptableObjects/Encounters/_EncounterDatabase.asset`.

Read it and edit **in place** — do not regenerate it and do not invent a GUID for the database asset.

`Entry` fields:
- `Encounter` — reference to your encounter `.asset`: `{fileID: 11400000, guid: <encounter guid>, type: 2}`.
- `Weight` (float ≥ 0) — relative chance in `Roll`. Actual probability is `Weight / sum(all eligible weights)`. Default `1`. Use `0` for an id-only encounter that should never be randomly rolled.
- `MinEncountersCompleted` (int ≥ 0) — battles the player must complete before this entry is eligible. Default `0`.

## Normal encounter — append to `Entries`

```yaml
- Encounter: {fileID: 11400000, guid: <encounter guid>, type: 2}
  Weight: <weight>
  MinEncountersCompleted: <min>
```

## Tutorial encounter — set the dedicated slot

Replace the `TutorialEncounter` line at the bottom of the database (it is always used for the player's first battle and excluded from `Roll`):

```yaml
TutorialEncounter: {fileID: 11400000, guid: <encounter guid>, type: 2}
```
