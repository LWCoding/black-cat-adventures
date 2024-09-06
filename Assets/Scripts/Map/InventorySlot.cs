using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class InventorySlot : MonoBehaviour
{

    [Header("Object Assignments")]
    [SerializeField] private Image _relicImage;
    [SerializeField] private TextMeshProUGUI _tooltipText;
    [Header("Draggable Icon (optional)")]
    [SerializeField] private UISlotDraggable _draggable;

    public Treasure CachedTreasure;

    public bool IsSlotUsed
    {
        set
        {
            // Automatically set whether the tooltip shows or not
            if (TryGetComponent(out UITooltipOnHover tooltipOnHover))
            {
                tooltipOnHover.IsInteractable = value;
            }
            _isSlotUsed = value;
        }
        get => _isSlotUsed;
    }
    private bool _isSlotUsed = false;

    private void Awake()
    {
        IsSlotUsed = false;
        _relicImage.enabled = false;  // Initially hide object until initialized
    }

    /// <summary>
    /// Initializes this inventory slot to display the correct
    /// relic and tooltip text.
    /// </summary>
    public void Initialize(Treasure treasure)
    {
        _relicImage.enabled = treasure != null;
        IsSlotUsed = treasure != null;
        CachedTreasure = treasure;
        if (treasure == null)
        {
            return;
        }
        _tooltipText.text = "<b><color=\"orange\">" + treasure.TreasureName + "</color></b>:\n" + treasure.TreasureDescription;
        _relicImage.sprite = treasure.TreasureIcon;
    }

    private void OnEnable()
    {
        if (_draggable != null)
        {
            _draggable.OnDragStart += InventorySlot_OnDragStart;
            _draggable.OnDragEnd += InventorySlot_OnDragEnd;
        }
    }

    private void OnDisable()
    {
        if (_draggable != null)
        {
            _draggable.OnDragStart -= InventorySlot_OnDragStart;
            _draggable.OnDragEnd -= InventorySlot_OnDragEnd;
        }
    }

    public void InventorySlot_OnDragStart()
    {
        // Hide tooltip if applicable
        if (TryGetComponent(out UITooltipOnHover tooltipOnHover))
        {
            IsSlotUsed = false;
            tooltipOnHover.OnPointerExit(null);
        }
    }

    public void InventorySlot_OnDragEnd(Vector3 placePosition)
    {
        // Reset tooltip's data
        if (TryGetComponent(out UITooltipOnHover tooltipOnHover))
        {
            IsSlotUsed = true;
        }
        // If we're colliding with another InventorySlot, swap these items
        PointerEventData pointerData = new(EventSystem.current)
        {
            pointerId = -1,
            position = Input.mousePosition
        };
        List<RaycastResult> results = new();
        EventSystem.current.RaycastAll(pointerData, results);

        if (results.Count != 0 && results[0].gameObject.transform.parent.TryGetComponent(out InventorySlot invSlot))
        {
            Treasure thisTreasure = CachedTreasure;
            // Take the other inventory slot's information if applicable
            Initialize(invSlot.CachedTreasure);
            // Make the other inventory slot absorb this information
            invSlot.Initialize(thisTreasure);
            // Hide tooltip if applicable
            if (invSlot.TryGetComponent(out tooltipOnHover))
            {
                tooltipOnHover.OnPointerEnter(null);
            }
        }
    }

}
