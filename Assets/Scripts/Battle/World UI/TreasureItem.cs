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

    private Treasure _treasureData;
    public Treasure TreasureData
    {
        get => _treasureData;
        private set
        {
            _treasureData = value;
        }
    }

    private Animator _animator;

    private void Awake()
    {
        _animator = GetComponent<Animator>();
    }

    /// <summary>
    /// Store an index into this treasure item, so that it can properly
    /// select itself. Also registers all of the treasure stats.
    /// </summary>
    public void Initialize(Treasure treasureData)
    {
        TreasureData = treasureData;
        _iconRenderer.sprite = TreasureData.TreasureIcon;
        _tooltipText.text = "<b>" + TreasureData.TreasureName + "</b>:\n" + TreasureData.TreasureDescription;
        TreasureData.ActivateTreasure();
        OnMouseExit();
    }

    public void OnMouseEnter()
    {
        EnableTreasure();
    }

    public void OnMouseExit()
    {
        DisableTreasure();
    }

    /// <summary>
    /// Make this treasure animate to its selected phase.
    /// </summary>
    public void EnableTreasure()
    {
        if (_animator == null)
        {
            _animator = GetComponent<Animator>();
        }
        _animator.Play("Selected");
    }

    /// <summary>
    /// Make this treasure animate to its unselected phase.
    /// </summary>
    public void DisableTreasure()
    {
        if (_animator == null)
        {
            _animator = GetComponent<Animator>();
        }
        _animator.Play("Unselected");
    }

}
