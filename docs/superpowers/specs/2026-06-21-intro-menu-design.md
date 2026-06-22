# Intro Menu Design

## Goal

Replace the auto-playing intro cutscene with a roguelike-style main menu offering **Start**, **Continue**, and **Quit**. The cutscene should only play when the player explicitly starts a new run.

## Current behavior (before this change)

- `IntroCutscene.cs` auto-runs in `Start()`: if a save exists, it immediately loads `Map`; otherwise it plays the cutscene animation and loads `Level1` afterward.
- `BypassIntro.cs` listens for Escape at all times in the Intro scene and jumps straight to `Level1`.

## New behavior

### `IntroMenuController.cs` (new script, `Assets/Scripts/Intro/`)

Serialized fields:
- `_startButton`, `_continueButton`, `_quitButton` (`Button`)
- `_confirmOverwritePanel` (`GameObject`) — "are you sure" popup shown when Start is clicked with an existing save
- `_confirmYesButton`, `_confirmNoButton` (`Button`)
- `_menuPanel` (`GameObject`) — the Start/Continue/Quit screen, hidden once a run begins
- `_introCutscene` (`IntroCutscene`)
- `_bypassIntro` (`BypassIntro`)

`Awake()`:
- Wire all five buttons' `onClick` via `AddListener` (no Inspector UnityEvent wiring, per explicit request).
- `bool hasSave = SaveManager.LoadGame() != null;` if false, `_continueButton.gameObject.SetActive(false)`.
- Hide `_confirmOverwritePanel`.

Handlers:
- `OnStartClicked()` — if a save exists, show `_confirmOverwritePanel`; otherwise call `BeginNewRun()`.
- `OnConfirmYesClicked()` — `SaveManager.EraseSave()`, `GameManager.GameData = new GameData()`, hide popup, call `BeginNewRun()`.
- `OnConfirmNoClicked()` — hide popup, remain on menu.
- `BeginNewRun()` — hide `_menuPanel`, set `_bypassIntro.IsActive = true`, call `_introCutscene.BeginCutscene()`.
- `OnContinueClicked()` — `GameManager.GameData = SaveManager.LoadGame()`, `SceneManager.LoadScene("Map")`.
- `OnQuitClicked()` — `Application.Quit()`.

### `IntroCutscene.cs` changes

- Remove `Start()` entirely (no more auto-play, no more save-check — both now live in `IntroMenuController`).
- Add a public method `BeginCutscene()` that starts `PlayCutsceneCoroutine()`. The coroutine body is unchanged: play "Play" animation, wait for click, play "Hide", then `SceneManager.LoadScene(_sceneNameAfterCutscene)`.

### `BypassIntro.cs` changes

- Add `public bool IsActive = false;`.
- `Update()` only checks for Escape when `IsActive` is `true`. This keeps Escape from skipping straight to `Level1` while the player is still on the menu screen; it becomes active only once `BeginNewRun()` starts the cutscene.

## Scope notes

- This is script-only. Building the actual Canvas/Button/popup GameObjects in `Intro.unity` and wiring SerializeField references is left to the user in the Editor.
- No changes to `SaveManager`, `GameData`, or scene names (`Map`, `Level1`) — all confirmed unchanged from current behavior.
