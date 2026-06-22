---
name: add-game-content
description: Scaffolds new enemies and new treasures for the Black Cat Adventures Unity project — writes the EnemyData/Treasure ScriptableObject .asset files, sprite GUID references, the duplicated enemy prefab, and (for treasures) the new C# effect class and its .meta file. Use this whenever the user asks to add a new enemy, boss, monster, foe, or describes a new treasure, item, collectible, relic, or battle ability/effect — even if they just describe the stats or behavior in plain language without using the word "skill" or "ScriptableObject". Always use this instead of hand-writing Unity .asset YAML from scratch.
---

# Add Game Content (enemies & treasures)

Black Cat Adventures represents enemies and treasures as Unity ScriptableObject `.asset` files that reference C# scripts and sprites by GUID, not by name or path. Getting a GUID wrong produces a "missing script"/broken reference in Unity with no compile error to catch it, so the mechanical parts of this process (finding GUIDs, writing YAML) must be done carefully and verified, not guessed.

## 0. Gather requirements first

Before writing anything, get from the user (ask if not given):
- **Name** of the enemy/treasure.
- **Flavor text** (`EnemyDescription` / `TreasureDescription`).
- **The effect**, in plain language: for an enemy, its health and attacks (name, damage, short description, any status effects inflicted); for a treasure, what it actually does mechanically.
- **Which already-imported images to use.** Never invent a filename or assume one exists — `Glob` `Assets/Images/**/*<keyword>*` to find candidates, or ask the user which file(s) they mean. If the needed image genuinely isn't imported yet, stop and tell the user to import it first; do not fabricate a placeholder reference.

Then branch:

- Adding an **enemy** → read [references/enemy.md](references/enemy.md) and follow it.
- Adding a **treasure** → read [references/treasure.md](references/treasure.md) and follow it.

Both reference files lean on the shared mechanics in [references/asset-mechanics.md](references/asset-mechanics.md) (how to look up a GUID, the `.meta` template for a brand-new script, the sprite sub-asset `fileID` rule) — read that first if anything about GUIDs/`fileID`s is unclear.

## Edge case: bespoke enemy mechanics

If the enemy needs unique runtime behavior beyond "has these stats and these attacks" (a boss with a special gimmick, a multi-phase fight, an attack that isn't just flat damage), that requires a custom `EnemyHandler` subclass, not just data. Don't try to force it into the data-only path — investigate `EnemyHandler` ([Assets/Scripts/Battle/Characters/EnemyHandler.cs](../../../Assets/Scripts/Battle/Characters/EnemyHandler.cs)) and `CharacterHandler` first, explain the extra scope to the user, and treat it as a small design task rather than pure scaffolding.

## Always finish with a manual verification step

This skill writes files; it cannot run the Unity Editor to confirm they import cleanly. After writing everything, tell the user explicitly:
1. Open the project in Unity and let it reimport.
2. Check the new `.asset` in the Inspector — every reference field should be populated, not "None"/"Missing".
3. For a new treasure script, confirm there's no "missing script" warning on the asset (this would mean the hand-written `.meta` GUID didn't match, or the C# has a compile error).
4. For a new enemy, place the duplicated prefab into the target scene and wire `_nextBattleObject` — this part is inherently a scene-editing task and is **not** done by this skill.
