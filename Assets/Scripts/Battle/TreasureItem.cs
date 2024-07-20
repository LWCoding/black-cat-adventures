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
    [Header("Treasure Properties")]
    public TreasureData TreasureData;

    private Animator _animator;

    private void Awake()
    {
        _animator = GetComponent<Animator>();
    }

    /// <summary>
    /// Store an index into this treasure item, so that it can properly
    /// select itself. Also registers all of the treasure stats.
    /// </summary>
    public void Initialize()
    {
        _iconRenderer.sprite = TreasureData.TreasureIcon;
        _tooltipText.text = "<b>" + TreasureData.TreasureName + "</b>:\n" + TreasureData.TreasureDescription;
        switch (TreasureData.Type)
        {
            case TreasureType.CAT_PAW:
                LevelManager.Instance.OnPlayerAttack += () =>
                {
                    LevelManager.Instance.DealDamageToEnemy(1);
                };
                break;
        }
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
        _animator.Play("Selected");
    }

    /// <summary>
    /// Make this treasure animate to its unselected phase.
    /// </summary>
    public void DisableTreasure()
    {
        _animator.Play("Unselected");
    }

}
