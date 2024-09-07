using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class TreasureSection : MonoBehaviour
{

    private static TreasureSection _instance;
    public static TreasureSection Instance
    {
        get
        {
            if (_instance == null)
            {
                _instance = FindAnyObjectByType<TreasureSection>();
            }
            return _instance;
        }
    }

    [Header("Object Assignments")]
    [SerializeField] private List<TreasureItem> _treasureObjects;
    [Header("Default Treasure Assignment")]
    [SerializeField] private Treasure _noneTreasure;

    public Action OnTreasureSelected = null;
    private bool _wasSectionInitialized = false;

    private void Awake()
    {
        if (_instance != this)
        {
            if (_instance != null)
            {
                Destroy(this);
            }
            else
            {
                _instance = this;
            }
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
            if (i >= GameManager.GameData.UnlockedTreasures.Count)
            {
                _treasureObjects[i].Initialize(_noneTreasure);
                continue;
            }
            _treasureObjects[i].Initialize(GameManager.GameData.UnlockedTreasures[i]);
        }
    }

    /// <summary>
    /// Given a TreasureType enum, returns True if that
    /// treasure is currently equipped, else False.
    /// 
    /// Returns False if section is still hidden.
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
