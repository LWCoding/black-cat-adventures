using System;
using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
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

    [Header("Selection Glow")]
    [Tooltip("Background sprite behind the keybind text that glows while this treasure is armed.")]
    [SerializeField] private SpriteRenderer _selectionGlowRenderer;
    [SerializeField] private Color _selectionGlowColor = new Color(1f, 0.85f, 0.2f, 1f);
    [SerializeField] private float _selectionGlowFadeDuration = 0.2f;

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
    private Color _glowOriginalColor;
    private bool _cachedGlowColor;
    private Tween _glowTween;

    private void Awake()
    {
        _animator = GetComponent<Animator>();
        _tooltip = GetComponent<TooltipOnHover>();
        CacheGlowColor();
    }

    private void CacheGlowColor()
    {
        if (_selectionGlowRenderer != null && !_cachedGlowColor)
        {
            _glowOriginalColor = _selectionGlowRenderer.color;
            _cachedGlowColor = true;
        }
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
        CacheGlowColor();
        ResetGlow();
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
                if (_keybindText != null && SlotIndex >= 0 && SlotIndex < TreasureSection.SlotKeyLabels.Length)
                {
                    _keybindText.text = TreasureSection.SlotKeyLabels[SlotIndex];
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

    /// <summary>Arms this slot: plays the Selected animation, glows the badge, and holds until resolved or cancelled.</summary>
    public void Arm()
    {
        _isArmed = true;
        if (_animator == null) { _animator = GetComponent<Animator>(); }
        _animator.Play("Selected");
        GlowTo(_selectionGlowColor);
    }

    /// <summary>Disarms this slot: plays the Unselected animation, fades the glow back, and clears the armed flag.</summary>
    public void Disarm()
    {
        _isArmed = false;
        if (_animator == null) { _animator = GetComponent<Animator>(); }
        _animator.Play("Unselected");
        GlowTo(_glowOriginalColor);
    }

    private void GlowTo(Color target)
    {
        if (_selectionGlowRenderer == null) { return; }
        _glowTween?.Kill();
        _glowTween = _selectionGlowRenderer.DOColor(target, _selectionGlowFadeDuration);
    }

    private void ResetGlow()
    {
        if (_selectionGlowRenderer == null) { return; }
        _glowTween?.Kill();
        _selectionGlowRenderer.color = _glowOriginalColor;
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

    /// <summary>
    /// Restores this slot's charges to full (called when a new enemy appears, so active
    /// treasures refresh once per enemy). No-op for passive/None slots.
    /// </summary>
    public void RefreshCharges()
    {
        if (_treasureData is not ActiveTreasure active) { return; }
        _chargesRemaining = active.MaxCharges;
        SetIconAlpha(1f);
        if (_keybindIndicator != null)
        {
            _keybindIndicator.SetActive(true);
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
