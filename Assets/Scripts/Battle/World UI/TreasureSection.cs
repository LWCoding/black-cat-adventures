using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using TMPro;
using UnityEngine;

public class TreasureSection : Singleton<TreasureSection>
{

    [Header("Object Assignments")]
    [SerializeField] private List<TreasureItem> _treasureObjects;
    [Header("Default Treasure Assignment")]
    [SerializeField] private Treasure _noneTreasure;
    [Header("Active Treasure Tutorial")]
    [SerializeField] private GameObject _activeTreasureTutorialTooltip;

    /// <summary>
    /// Keybind for each treasure slot, indexed by slot. Single source of truth shared by
    /// BattleManager (input) and TreasureItem (on-screen badge). Rebind here to change both.
    /// </summary>
    public static readonly KeyCode[] SlotKeyCodes =
    {
        KeyCode.F1, KeyCode.F2, KeyCode.F3, KeyCode.F4, KeyCode.F5,
    };

    /// <summary>Display labels for each slot's keybind, matching <see cref="SlotKeyCodes"/> by index.</summary>
    public static readonly string[] SlotKeyLabels =
    {
        "F1", "F2", "F3", "F4", "F5",
    };

    public Action OnTreasureSelected = null;
    private bool _wasSectionInitialized = false;
    private bool _checkedActiveTutorial = false;

    /// <summary>The slot that is currently armed and awaiting a tile target, or null.</summary>
    private TreasureItem _armedItem;
    public bool IsAwaitingTarget => _armedItem != null;

    /// <summary>
    /// An instant treasure that has been selected via its keybind and is awaiting a
    /// confirmation press before it actually triggers, or null. This lets the player
    /// read its tooltip first instead of accidentally spending a one-shot treasure.
    /// </summary>
    private TreasureItem _pendingItem;

    /// <summary>True when a treasure is either armed for targeting or awaiting use-confirmation.</summary>
    public bool HasActiveSelection => _armedItem != null || _pendingItem != null;

    protected override void Awake()
    {
        base.Awake();
        _activeTreasureTutorialTooltip?.SetActive(false);
        // Auto-cancel targeting whenever the player's turn ends, and check whether
        // to show the active-treasure tutorial on the first player turn.
        BattleManager.Instance.OnStateChanged += OnStateChanged;
        // Active treasures refresh their charges once per enemy.
        BattleManager.Instance.OnNewEnemySet += (_) => RefreshAllCharges();
    }

    private void OnDisable()
    {
        if (BattleManager.Instance != null)
        {
            BattleManager.Instance.OnStateChanged -= OnStateChanged;
        }
        SubmitButton.OnClickButton -= HideActiveTreasureTutorial;
    }

    private void OnStateChanged(State newState)
    {
        if (newState is not PlayerTurnState)
        {
            CancelSelection();
            return;
        }
        if (!_checkedActiveTutorial)
        {
            _checkedActiveTutorial = true;
            TryShowActiveTreasureTutorial();
        }
    }

    private void TryShowActiveTreasureTutorial()
    {
        if (_activeTreasureTutorialTooltip == null) { return; }
        if (GameManager.GameData.HasSeenActiveTreasureTutorial) { return; }
        if (!gameObject.activeSelf) { return; }
        if (!GameManager.GameData.EquippedTreasures.Any(t => t is ActiveTreasure)) { return; }

        _activeTreasureTutorialTooltip.SetActive(true);
        GameManager.GameData.HasSeenActiveTreasureTutorial = true;
        SaveManager.SaveGame(GameManager.GameData);
        SubmitButton.OnClickButton += HideActiveTreasureTutorial;
    }

    private void HideActiveTreasureTutorial()
    {
        _activeTreasureTutorialTooltip?.SetActive(false);
        SubmitButton.OnClickButton -= HideActiveTreasureTutorial;
    }

    /// <summary>Cancels any pending selection and restores every slot's charges to full.</summary>
    private void RefreshAllCharges()
    {
        CancelSelection();
        foreach (TreasureItem item in _treasureObjects)
        {
            item.RefreshCharges();
        }
    }

    private void Start()
    {
        // If we haven't set the treasures yet, initialize their effects
        if (!_wasSectionInitialized)
        {
            Initialize();
            _wasSectionInitialized = true;
        }
    }

    private void Initialize()
    {
        // Initialize all items inside
        for (int i = 0; i < _treasureObjects.Count; i++)
        {
            Treasure treasure = i < GameManager.GameData.UnlockedTreasures.Count
                ? GameManager.GameData.UnlockedTreasures[i]
                : _noneTreasure;
            _treasureObjects[i].Initialize(treasure, i);
        }
    }

    /// <summary>
    /// Attempts to select/trigger the treasure at the given slot index (0-based).
    /// No-ops if it is not the player's turn, the game is paused, the index is
    /// out of range, or the slot is not triggerable.
    ///
    /// Behaviour depends on the treasure and <paramref name="requireConfirm"/>:
    /// - Treasures that need a tile target arm and wait for a tile click (toggling the
    ///   same slot again cancels).
    /// - Instant treasures with <paramref name="requireConfirm"/> true (keybind use)
    ///   are selected on the first press (revealing their tooltip) and only triggered
    ///   on a second press of the same slot, so the player can't accidentally spend a
    ///   one-shot treasure they only wanted to read.
    /// - Instant treasures with <paramref name="requireConfirm"/> false (mouse click,
    ///   which already shows the tooltip on hover) trigger immediately.
    /// </summary>
    public void TryTriggerSlot(int index, bool requireConfirm = true)
    {
        if (BattleManager.Instance == null) { return; }
        if (BattleManager.Instance.CurrentState is not PlayerTurnState) { return; }
        if (PauseMenuManager.Instance != null && PauseMenuManager.Instance.IsPaused) { return; }
        if (index < 0 || index >= _treasureObjects.Count) { return; }

        TreasureItem item = _treasureObjects[index];

        // Toggle off if the same slot is already armed for targeting.
        if (_armedItem == item)
        {
            CancelTargeting();
            return;
        }

        // If a different slot is already armed for targeting, cancel it first.
        if (_armedItem != null)
        {
            CancelTargeting();
        }

        if (!item.IsTriggerable) { return; }

        ActiveTreasure active = (ActiveTreasure)item.TreasureData;

        // Treasures that require a follow-up tile selection arm and wait for the click.
        if (active.RequiresTileTarget)
        {
            ClearPendingSelection();
            HideActiveTreasureTutorial();
            item.Arm();
            OnTreasureSelected?.Invoke();
            bool resolvedImmediately = active.OnTrigger();
            if (resolvedImmediately)
            {
                item.ConsumeCharge();
                item.Disarm();
            }
            else
            {
                _armedItem = item;
            }
            return;
        }

        // Instant treasures triggered via keybind: first press selects + reveals the
        // tooltip, second press of the same slot commits to using it.
        if (requireConfirm && _pendingItem != item)
        {
            SelectPending(item);
            return;
        }

        // Either the confirmation press, or a direct (mouse-click) use: trigger now.
        UseImmediate(item, active);
    }

    /// <summary>
    /// Selects an instant treasure for confirmation: highlights it and reveals its
    /// tooltip without triggering it yet. Switches selection off any previous slot.
    /// </summary>
    private void SelectPending(TreasureItem item)
    {
        if (_pendingItem != null && _pendingItem != item)
        {
            _pendingItem.Disarm();
            _pendingItem.HideTooltip();
        }
        _pendingItem = item;
        HideActiveTreasureTutorial();
        item.Arm();
        item.ShowTooltip();
        OnTreasureSelected?.Invoke();
    }

    /// <summary>
    /// Triggers an instant treasure now and resets its visuals. Deliberately does not
    /// call Arm() first, so it never plays Selected then Unselected in the same frame
    /// (which would leave the slot stuck in its expanded state).
    /// </summary>
    private void UseImmediate(TreasureItem item, ActiveTreasure active)
    {
        // Clear a different slot's pending selection so it doesn't stay highlighted.
        if (_pendingItem != null && _pendingItem != item)
        {
            _pendingItem.Disarm();
            _pendingItem.HideTooltip();
        }
        _pendingItem = null;
        item.HideTooltip();
        HideActiveTreasureTutorial();
        OnTreasureSelected?.Invoke();

        bool resolvedImmediately = active.OnTrigger();
        if (resolvedImmediately)
        {
            item.ConsumeCharge();
            item.Disarm();
        }
        else
        {
            // A custom instant treasure chose to wait for a tile after all.
            item.Arm();
            _armedItem = item;
        }
    }

    /// <summary>Clears any pending (keybind-selected) treasure, restoring its visuals.</summary>
    private void ClearPendingSelection()
    {
        if (_pendingItem == null) { return; }
        _pendingItem.Disarm();
        _pendingItem.HideTooltip();
        _pendingItem = null;
    }

    /// <summary>Cancels both targeting and pending use-confirmation selections.</summary>
    public void CancelSelection()
    {
        CancelTargeting();
        ClearPendingSelection();
    }

    /// <summary>
    /// Forwards a tile click to the currently armed treasure.
    /// Returns true if the click was consumed (whether or not the target was valid),
    /// so callers can skip normal tile-selection logic.
    /// </summary>
    public bool HandleTileTargeted(LetterTile tile)
    {
        if (_armedItem == null) { return false; }

        ActiveTreasure active = (ActiveTreasure)_armedItem.TreasureData;
        bool accepted = active.OnTileTargeted(tile);
        if (accepted)
        {
            _armedItem.ConsumeCharge();
            _armedItem.Disarm();
            _armedItem = null;
        }
        // Return true even if rejected: the click was intercepted so normal
        // tile-selection (word building) should not happen.
        return true;
    }

    /// <summary>Cancels the currently armed treasure, restoring normal tile interaction.</summary>
    public void CancelTargeting()
    {
        if (_armedItem == null) { return; }
        _armedItem.Disarm();
        _armedItem = null;
    }

    /// <summary>
    /// Given a TreasureType, returns true if that treasure is currently equipped.
    /// Returns false if the section is still hidden (e.g. tutorial).
    /// </summary>
    public bool HasTreasure(Type treasure)
    {
        if (!gameObject.activeSelf) { return false; }
        // If we haven't set the treasures yet, initialize their effects
        if (!_wasSectionInitialized)
        {
            Initialize();
            _wasSectionInitialized = true;
        }
        foreach (TreasureItem item in _treasureObjects)
        {
            if (item.TreasureData.GetType() == treasure)
            {
                return true;
            }
        }
        return false;
    }

}
