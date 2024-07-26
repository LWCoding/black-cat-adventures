using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class TreasureSection : MonoBehaviour
{

    public static bool UnlockedTreasureSection = false;

    public static TreasureSection Instance;

    [Header("Object Assignments")]
    [SerializeField] private List<TreasureItem> _treasureObjects;

    public Action OnTreasureSelected = null;

    private void Awake()
    {
        if (Instance != null)
        {
            Destroy(this);
        }
        Instance = this;
    }

    private void OnEnable()
    {
        // If we haven't unlocked this yet, hide it
        if (!UnlockedTreasureSection)
        {
            gameObject.SetActive(false);
            return;
        }
        // Initialize all items inside
        foreach (TreasureItem item in _treasureObjects)
        {
            item.Initialize();
        }
    }

    /// <summary>
    /// Given a TreasureType enum, returns True if that
    /// treasure is currently equipped, else False.
    /// </summary>
    public bool HasTreasure(TreasureType type)
    {
        foreach (TreasureItem item in _treasureObjects)
        {
            if (item.TreasureData.Type == type)
            {
                return true;
            }
        }
        return false;
    }

}
