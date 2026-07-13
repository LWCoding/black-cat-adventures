using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class TreasureSection : Singleton<TreasureSection>
{

    [Header("Object Assignments")]
    [SerializeField] private List<TreasureItem> _treasureObjects;
    [Header("Default Treasure Assignment")]
    [SerializeField] private Treasure _noneTreasure;

    /// <summary>
    /// Keybind for each treasure slot, indexed by slot. Single source of truth shared by
    /// BattleManager (input) and TreasureItem (on-screen badge). Rebind here to change both.
    /// </summary>
    public static readonly KeyCode[] SlotKeyCodes =
    {
        KeyCode.LeftBracket, KeyCode.RightBracket, KeyCode.Backslash, KeyCode.Semicolon, KeyCode.Quote,
    };

    /// <summary>Display labels for each slot's keybind, matching <see cref="SlotKeyCodes"/> by index.</summary>
    public static readonly string[] SlotKeyLabels =
    {
        "[", "]", "\\", ";", "'",
    };

    public Action OnTreasureSelected = null;
    private bool _wasSectionInitialized = false;

    /// <summary>The slot that is currently armed and awaiting a tile target, or null.</summary>
    private TreasureItem _armedItem;
    public bool IsAwaitingTarget => _armedItem != null;

    protected override void Awake()
    {
        base.Awake();
        // Auto-cancel targeting whenever the player's turn ends.
        BattleManager.Instance.OnStateChanged += (newState) =>
        {
            if (newState is not PlayerTurnState)
            {
                CancelTargeting();
            }
        };
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
    /// Attempts to trigger the treasure at the given slot index (0-based).
    /// No-ops if it is not the player's turn, the game is paused, the index is
    /// out of range, or the slot is not triggerable.
    /// Toggling the currently-armed slot cancels targeting instead.
    /// </summary>
    public void TryTriggerSlot(int index)
    {
        if (BattleManager.Instance == null) { return; }
        if (BattleManager.Instance.CurrentState is not PlayerTurnState) { return; }
        if (PauseMenuManager.Instance != null && PauseMenuManager.Instance.IsPaused) { return; }
        if (index < 0 || index >= _treasureObjects.Count) { return; }

        TreasureItem item = _treasureObjects[index];

        // Toggle off if the same slot is already armed.
        if (_armedItem == item)
        {
            CancelTargeting();
            return;
        }

        // If a different slot is already armed, cancel it first.
        if (_armedItem != null)
        {
            CancelTargeting();
        }

        if (!item.IsTriggerable) { return; }

        item.Arm();
        OnTreasureSelected?.Invoke();

        ActiveTreasure active = (ActiveTreasure)item.TreasureData;
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
