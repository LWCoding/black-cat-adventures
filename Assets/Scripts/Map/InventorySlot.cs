using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class InventorySlot : MonoBehaviour
{

    [Header("Object Assignments")]
    [SerializeField] private Image _relicImage;
    [SerializeField] private TextMeshProUGUI _tooltipText;

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
        _relicImage.enabled = true;
        _tooltipText.text = "<b><color=\"orange\">" + treasure.TreasureName + "</color></b>:\n" + treasure.TreasureDescription;
        _relicImage.sprite = treasure.TreasureIcon;
        IsSlotUsed = true;
    }

}
