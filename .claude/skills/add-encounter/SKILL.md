---
name: add-encounter
description: Scaffolds a new encounter (a sequence of enemies the player fights back-to-back in one battle) for the Black Cat Adventures Unity project — creates each new enemy's EnemyData via the add-enemy flow, writes the Encounter ScriptableObject .asset with one EnemySpawn per enemy (dialogue, timing, spawn offset), and registers the encounter in _EncounterDatabase.asset. Use whenever the user asks to add a new encounter, battle, fight, wave, or group/series/squad of enemies, or describes "a level where you fight X then Y then Z". Always use this instead of hand-writing Encounter YAML or editing the database by hand.
---

# Add Encounter

An **encounter** is a single battle made up of a series of enemies the player fights one after another. In Black Cat Adventures it's an `Encounter` ScriptableObject (`Assets/Scripts/Battle/Levels/Encounter.cs`, `.asset` GUID-references the enemies) holding an ordered `List<EnemySpawn>`. At runtime `LevelSpawner` reads the encounter (looked up by id, or rolled from `_EncounterDatabase`), spawns each enemy from a **single shared `Enemy.prefab`**, and chains them via `SetNextBattleObject`. After the last enemy a treasure chest is appended automatically.

Because spawning is data-driven, **enemies in an encounter are pure data** — you do NOT duplicate `Enemy.prefab` per enemy or place anything in a scene (that older workflow is obsolete in the encounter system). You only need each enemy's `EnemyData` `.asset`.

Like all assets here, everything references C# scripts and other assets by **GUID**. A wrong GUID produces a silent "missing reference" in Unity with no compile error. Read [references/encounter.md](references/encounter.md) for the exact YAML templates and GUID lookup rules.

## 0. Gather requirements first

Before writing anything, get from the user (ask if not given):

1. **Encounter name / id** — a short stable string (e.g. `"Thieves"`, `"OceanAmbush"`). This becomes both the asset filename and the `EncounterId` field. Confirm it doesn't already exist by `Glob`bing `Assets/Resources/ScriptableObjects/Encounters/*.asset`.

2. **The ordered list of enemies.** For each enemy in fight order:
   - Is it an **existing** enemy or a **new** one? Existing → you'll look up its `EnemyData` `.asset` GUID. New → you'll create it (see step 1).
   - Its **intro dialogue** (`DialogueToPlayOnMeet`), if any — the lines shown when the player reaches that enemy. Each line has text, who says it (player or enemy), and whether it pauses the battle. Ask for the lines; an enemy can have zero.
   - Optional per-enemy tuning: `ShouldStallBeforeTurn`, `TimeToNextObject` (default `1.3`), `SpawnOffset` (default `{x: 0, y: 0}`). Use the defaults unless the user cares.

3. **How the encounter is reached:**
   - Is this the **tutorial encounter** (always used for the player's very first battle)? If yes, it goes in the database's `TutorialEncounter` slot and is excluded from random rolls.
   - Otherwise it's a normal entry in `_EncounterDatabase`: ask for its **Weight** (relative selection chance, default `1`) and **MinEncountersCompleted** (how many battles the player must finish before it can be rolled, default `0`).
   - Or: should it be reachable only by **id** (e.g. a specific map node hands off `EncounterId`) and never randomly rolled? Then still add it to the database (so `GetEncounter` finds it) but the user may want Weight `0`. Clarify intent.

## 1. Create each NEW enemy (data only)

For every enemy in the list that doesn't exist yet, use the **add-enemy** skill to create its `EnemyData` `.asset`. That skill handles sprite resolution and asset writing; skip the prefab/scene-placement steps it describes — the encounter system spawns from one shared `Enemy.prefab` and chains enemies in code.

Do **not** hand-write `.meta` files. Unity generates them automatically on reimport. For **existing** enemies, read their `Assets/Resources/ScriptableObjects/Characters/<Name>.asset.meta` to get the GUID (never guess it). For new enemies their GUID won't be known until after Unity imports the file; write the encounter asset with a placeholder GUID and update via the Inspector on first open.

If an enemy needs bespoke runtime behavior beyond stats + attacks, that's an `EnemyHandler` subclass — see the "bespoke enemy mechanics" note in [../add-game-content/SKILL.md](../add-game-content/SKILL.md); handle it as a design task, not pure scaffolding.

## 2. Write the Encounter `.asset`

Path: `Assets/Resources/ScriptableObjects/Encounters/<EncounterName>.asset`

Follow the exact template in [references/encounter.md](references/encounter.md). Key points:
- `m_Script` GUID is always `Encounter`'s GUID — re-confirm from `Assets/Scripts/Battle/Levels/Encounter.cs.meta` (currently `0f09b01bdfcd89596d2250fbbae52cc1`).
- `EncounterId` = the stable string from step 0.
- `Enemies` is an ordered YAML list — one `EnemySpawn` block per enemy, in fight order. Each block's `EnemyData` is `{fileID: 11400000, guid: <that enemy's EnemyData guid>, type: 2}`.

## 3. Register in `_EncounterDatabase.asset`

Do **not** hand-write the `.asset.meta` — Unity generates it on reimport. Because the new encounter's GUID isn't known until after import, register the encounter via the Unity Inspector rather than by editing the YAML directly:

1. Open the project in Unity and let it reimport so the new asset gets a GUID.
2. Select `Assets/Resources/ScriptableObjects/Encounters/_EncounterDatabase.asset` in the Project panel.
3. In the Inspector:
   - **Normal encounter**: click **+** on the `Entries` list, drag the new `.asset` into the `Encounter` slot, and set `Weight` and `MinEncountersCompleted`.
   - **Tutorial encounter**: drag the new `.asset` into the `TutorialEncounter` slot.

If you need to YAML-edit the database directly (e.g. for automation), first read the newly-generated `.meta` to get the GUID, then append to `Entries` using the template in [references/encounter.md](references/encounter.md).

## Always finish with a manual verification checklist

This skill writes files; it cannot run the Unity Editor. After writing everything, tell the user explicitly:

1. Open the project in Unity and let it reimport.
2. Select the new `<EncounterName>.asset`. In the Inspector confirm: no "Missing Script" warning, `EncounterId` is correct, and every `EnemySpawn`'s `EnemyData` is populated (not "None"/"Missing") and in the intended order. If any `EnemyData` shows "None" (because the GUID was left as a placeholder), drag the correct asset in now.
3. For each newly created enemy, open its `EnemyData` `.asset` and confirm all sprite/attack references are populated (same checks as the add-enemy skill).
4. Register the encounter in `_EncounterDatabase.asset` via the Inspector (see step 3 above). Confirm the new entry appears with the right `Weight`/`MinEncountersCompleted`, or in the `TutorialEncounter` slot.
5. Enter Play mode and reach this encounter (via its map node / id, or by forcing a roll) — confirm the enemies spawn in order with the right dialogue, and that a treasure chest appears after the last enemy.
