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
    [SerializeField] private TextMeshPro _treasureDesc;

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
        // Select the first treasure item, and initialize all items
        SelectTreasure(0);
        int idx = 0;
        foreach (TreasureItem item in _treasureObjects)
        {
            item.Initialize(idx++);
        }
    }

    public void SelectTreasure(int idx)
    {
        if (idx < 0 || idx >= _treasureObjects.Count) { return; }
        for (int i = 0; i < _treasureObjects.Count; i++)
        {
            if (i == idx) { continue; }
            _treasureObjects[i].DisableTreasure();
        }
        _treasureObjects[idx].EnableTreasure();
        _treasureDesc.text = "<b>" + _treasureObjects[idx].TreasureData.TreasureName + "</b>\n\n" + _treasureObjects[idx].TreasureData.TreasureDescription;
    }

}
