using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

[RequireComponent(typeof(Animator))]
public class TreasureItem : MonoBehaviour
{

    [Header("Object Assignments")]
    [SerializeField] private SpriteRenderer _iconRenderer;
    [SerializeField] private TextMeshPro _tooltipText;
    [SerializeField] private GameObject _keybindIndicator;
    [SerializeField] private TextMeshPro _keybindText;

    private Treasure _treasureData;
    public Treasure TreasureData
    {
        get => _treasureData;
        private set
        {
            _treasureData = value;
        }
    }

    public int SlotIndex { get; private set; }

    private int _chargesRemaining;
    private bool _isArmed;
    private Animator _animator;
    private TooltipOnHover _tooltip;

    private void Awake()
    {
        _animator = GetComponent<Animator>();
        _tooltip = GetComponent<TooltipOnHover>();
    }

    /// <summary>
    /// Initializes this slot with a treasure and its position index (0-based).
    /// Registers passive/active effects, shows the keybind badge for active treasures.
    /// </summary>
    public void Initialize(Treasure treasureData, int slotIndex)
    {
        SlotIndex = slotIndex;
        TreasureData = treasureData;
        _isArmed = false;
        _iconRenderer.sprite = TreasureData.TreasureIcon;
        SetIconAlpha(1f);
        _tooltipText.text = "<b><color=#" + TreasureRarityInfo.GetHexColor(TreasureData.Rarity) + ">" + TreasureData.TreasureName + "</color></b>:\n" + TreasureData.TreasureDescription;

        // Empty slots (None placeholder) should not reveal a tooltip on hover.
        if (_tooltip != null)
        {
            _tooltip.enabled = treasureData is not None;
        }

        // Active treasures get a keybind badge and charge tracking.
        if (treasureData is ActiveTreasure active)
        {
            _chargesRemaining = active.MaxCharges;
            if (_keybindIndicator != null)
            {
                _keybindIndicator.SetActive(true);
                if (_keybindText != null)
                {
                    _keybindText.text = (SlotIndex + 1).ToString();
                }
            }
        }
        else
        {
            _chargesRemaining = 0;
            if (_keybindIndicator != null)
            {
                _keybindIndicator.SetActive(false);
            }
        }

        TreasureData.ActivateTreasure();
        OnMouseExit();
    }

    /// <summary>
    /// Whether this slot can currently be triggered by the player.
    /// Requires an ActiveTreasure with charges remaining that also passes its own CanTrigger check.
    /// </summary>
    public bool IsTriggerable =>
        _treasureData is ActiveTreasure active
        && _chargesRemaining > 0
        && active.CanTrigger();

    /// <summary>Arms this slot: plays the Selected animation and holds it until resolved or cancelled.</summary>
    public void Arm()
    {
        _isArmed = true;
        if (_animator == null) { _animator = GetComponent<Animator>(); }
        _animator.Play("Selected");
    }

    /// <summary>Disarms this slot: plays the Unselected animation and clears the armed flag.</summary>
    public void Disarm()
    {
        _isArmed = false;
        if (_animator == null) { _animator = GetComponent<Animator>(); }
        _animator.Play("Unselected");
    }

    /// <summary>
    /// Decrements remaining charges. When exhausted, dims the icon and hides the keybind badge.
    /// </summary>
    public void ConsumeCharge()
    {
        _chargesRemaining = Mathf.Max(0, _chargesRemaining - 1);
        if (_chargesRemaining == 0)
        {
            SetIconAlpha(0.35f);
            if (_keybindIndicator != null)
            {
                _keybindIndicator.SetActive(false);
            }
        }
    }

    public void OnMouseEnter()
    {
        // Empty slots and passive slots don't light up or show tooltips.
        if (_treasureData is None) { return; }
        if (!_isArmed)
        {
            EnableTreasure();
        }
    }

    public void OnMouseExit()
    {
        // Stay lit while armed.
        if (_isArmed) { return; }
        DisableTreasure();
    }

    private void OnMouseDown()
    {
        if (_treasureData is ActiveTreasure && TreasureSection.Instance != null)
        {
            TreasureSection.Instance.TryTriggerSlot(SlotIndex);
        }
    }

    /// <summary>Make this treasure animate to its selected (lit) phase.</summary>
    public void EnableTreasure()
    {
        if (_animator == null) { _animator = GetComponent<Animator>(); }
        _animator.Play("Selected");
    }

    /// <summary>Make this treasure animate to its unselected (dim) phase.</summary>
    public void DisableTreasure()
    {
        if (_animator == null) { _animator = GetComponent<Animator>(); }
        _animator.Play("Unselected");
    }

    private void SetIconAlpha(float alpha)
    {
        Color c = _iconRenderer.color;
        c.a = alpha;
        _iconRenderer.color = c;
    }

}
