using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering;

public class InventoryManager : MonoBehaviour
{

    [Header("Equipped Slots")]
    [SerializeField] private List<InventorySlot> _equippedSlots;
    [Header("Object Assignments")]
    [SerializeField] private GameObject _allSlotsObject;

    private List<InventorySlot> _allSlots = new();

    private void Awake()
    {
        _allSlotsObject.SetActive(false);
        // Loop over all additional slots, cache them for storage purposes
        for (int i = 0; i < _allSlotsObject.transform.childCount; i++)
        {
            if (_allSlotsObject.transform.GetChild(i).TryGetComponent(out InventorySlot iSlot))
            {
                _allSlots.Add(iSlot);
            }
        }
    }

    /// <summary>
    /// When we leave this scene, set the treasures to the
    /// equipped treasures.
    /// </summary>
    private void OnDisable()
    {
        List<Treasure> equipped = new();
        List<Treasure> other = new();
        foreach (InventorySlot slot in _equippedSlots)
        {
            if (slot.CachedTreasure != null)
            {
                equipped.Add(slot.CachedTreasure);
            }
        }
        foreach (InventorySlot slot in _allSlots)
        {
            if (slot.CachedTreasure != null)
            {
                other.Add(slot.CachedTreasure);
            }
        }
        equipped.AddRange(other);
        GameManager.UnlockedTreasures = equipped;
    }

    private void Start()
    {
        // Initialize all equipped slots to equipped treasures
        for (int i = 0; i < GameManager.UnlockedTreasures.Count; i++)
        {
            if (i < _equippedSlots.Count)
            {
                _equippedSlots[i].Initialize(GameManager.UnlockedTreasures[i]);
            } 
            else
            {
                _allSlots[i - 3].Initialize(GameManager.UnlockedTreasures[i]);
            }
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
