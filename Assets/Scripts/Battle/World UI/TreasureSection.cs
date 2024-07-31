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

    private void Start()
    {
        // If we haven't unlocked this yet, hide it
        if (!UnlockedTreasureSection)
        {
            gameObject.SetActive(false);
        }
    }

    private void OnEnable()
    {
        if (!UnlockedTreasureSection) { return; }
        // Initialize all items inside
        foreach (TreasureItem item in _treasureObjects)
        {
            item.Initialize();
        }
    }

    /// <summary>
    /// Given a TreasureType enum, returns True if that
    /// treasure is currently equipped, else False.
    /// 
    /// Returns false if the section is locked.
    /// </summary>
    public bool HasTreasure(Type treasure)
    {
        if (!UnlockedTreasureSection) { return false; }
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
