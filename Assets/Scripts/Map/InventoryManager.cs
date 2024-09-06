using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InventoryManager : MonoBehaviour
{

    [Header("Equipped Slots")]
    [SerializeField] private List<InventorySlot> _inventorySlots;
    [Header("Object Assignments")]
    [SerializeField] private GameObject _allSlotsObject;

    private void Awake()
    {
        _allSlotsObject.SetActive(false);
    }

    /// <summary>
    /// When we leave this scene, set the treasures to the
    /// equipped treasures.
    /// </summary>
    private void OnDisable()
    {
        List<Treasure> equipped = new();
        foreach (InventorySlot slot in _inventorySlots)
        {
            if (slot.CachedTreasure != null)
            {
                equipped.Add(slot.CachedTreasure);
            }
        }
        GameManager.EquippedTreasures = equipped;
    }

    private void Start()
    {
        // Initialize all equipped slots to equipped treasures
        for (int i = 0; i < GameManager.EquippedTreasures.Count; i++)
        {
            _inventorySlots[i].Initialize(GameManager.EquippedTreasures[i]);
        }
    }

    /// <summary>
    /// Toggle the visibility of the inventory from on -> off or off -> on.
    /// </summary>
    public void ToggleInventoryVisibility()
    {
        SetInventoryVisibility(!_allSlotsObject.activeSelf);
    }

    /// <summary>
    /// Toggle whether or not the extra slots for the inventory are
    /// shown or not.
    /// </summary>
    public void SetInventoryVisibility(bool isShown)
    {
        _allSlotsObject.SetActive(isShown);
    }

}
