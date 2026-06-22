# Shared Unity asset mechanics

Both enemy and treasure assets are Unity `MonoBehaviour` ScriptableObject `.asset` files in YAML. They reference C# scripts and sprites by **GUID**, recorded in each asset's exact-same-name `.meta` file. There is no way to derive a GUID from a name or path — it must be read from the actual `.meta` file on disk, or (for a brand-new script) generated.

## Looking up a script's GUID

For any *existing* script (e.g. `EnemyData`, `Treasure`, or any concrete treasure class you're copying the pattern from), read its `.meta` file — it's a plain sibling file:

```
Assets/Scripts/Battle/Characters/EnemyData.cs.meta
```

The `guid:` line is what you need. For a new enemy, the script you reference is always the same: `EnemyData` at `Assets/Scripts/Battle/Characters/EnemyData.cs`, confirmed GUID `cf72fda75ee21d9469e20180c8adc802`. For a new treasure, you're writing a brand-new concrete subclass, so there is no existing GUID to look up for *that* script — see the "brand-new script" section below. The abstract `Treasure` base class's own GUID is never referenced directly by anything (an asset's `m_Script` field points at the concrete subclass, not the base class).

Don't hardcode/reuse GUIDs across sessions for anything other than the `EnemyData` anchor above — always re-read the `.meta` file fresh, since scripts can be deleted/recreated (which changes their GUID).

## Looking up a sprite's GUID

For a PNG already imported into the project (e.g. `Assets/Images/Characters/ArcFoxIdle.png`), read its `.meta` file:

```
Assets/Images/Characters/ArcFoxIdle.png.meta
```

Two things matter:
1. The `guid:` line at the top — this is the texture's GUID.
2. The `spriteMode:` line under `TextureImporter`. **`spriteMode: 1` means "Single"** — every sprite/icon in this project uses single mode. In that case, the Sprite sub-asset reference to use in any `.asset`/prefab is:
   ```yaml
   {fileID: 21300000, guid: <the texture's guid>, type: 3}
   ```
   `21300000` is Unity's fixed sub-asset ID for the one Sprite generated from a single-sprite-mode texture — it does not vary per file. If you ever encounter `spriteMode: 2` (Multiple/sliced sheet), this fixed-fileID trick does *not* hold — stop and ask for guidance rather than guessing, since each sub-sprite gets its own generated fileID recorded under `spriteSheet.sprites` in the `.meta`.

Never invent a GUID for an existing asset. If the user names an image that doesn't exist yet, tell them to import it first.

## Writing a `.meta` for a brand-new C# script

A new `Treasure` subclass (or any new script) doesn't have a Unity-assigned GUID until the Editor imports it. Since this skill writes files outside the Editor, generate the GUID yourself and write the `.meta` by hand so the `.asset` file can reference it immediately — this is the exact format Unity itself writes for a `MonoImporter`:

```yaml
fileFormatVersion: 2
guid: <32 lowercase hex chars, no dashes>
MonoImporter:
  externalObjects: {}
  serializedVersion: 2
  defaultReferences: []
  executionOrder: 0
  icon: {instanceID: 0}
  userData: 
  assetBundleName: 
  assetBundleVariant: 
```

Generate the GUID with:

```powershell
[guid]::NewGuid().ToString("N")
```

(`"N"` formats it as 32 hex chars with no dashes/braces — matching Unity's format exactly.) Save the generated value before writing the `.meta` and `.asset` — they must use the *identical* string.

This is safe because the chance of a random GUID colliding with one already in the project is astronomically small, and Unity will accept a pre-existing `.meta` next to a script on import rather than generating a new one — this is the same mechanism asset bundles/package tooling rely on. Still, the Unity Editor open-and-reimport step in the main SKILL.md is what actually confirms it worked; don't skip it.

## Writing a `.meta` for a brand-new `.asset` file (EnemyData / Treasure data)

Every new `EnemyData` or `Treasure` `.asset` you write also needs its own sibling `.meta` — this one uses `NativeFormatImporter`, not `MonoImporter` (confirmed from `ArcFox.asset.meta` and `Fools Gold.asset.meta`):

```yaml
fileFormatVersion: 2
guid: <32 lowercase hex chars, no dashes — a fresh one, distinct from the script's GUID>
NativeFormatImporter:
  externalObjects: {}
  mainObjectFileID: 11400000
  userData: 
  assetBundleName: 
  assetBundleVariant: 
```

`mainObjectFileID: 11400000` matches the `--- !u!114 &11400000` header at the top of the `.asset` YAML itself — every `EnemyData`/`Treasure` asset uses this same fileID for its single root object, so leave it as-is. Generate the GUID the same way as for scripts, and this is the GUID you then reference wherever something points *at* this asset (e.g. an enemy prefab's `_charData` field, or `TreasureCollectible`/unlock lists if the user is also wiring up how the treasure is obtained).
