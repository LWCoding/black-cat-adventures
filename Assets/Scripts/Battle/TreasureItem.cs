using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Animator))]
public class TreasureItem : MonoBehaviour
{

    [Header("Object Assignments")]
    [SerializeField] private SpriteRenderer _iconRenderer;
    [Header("Treasure Properties")]
    public TreasureData TreasureData;

    private Animator _animator;
    private int _savedIdx;

    private void Awake()
    {
        _animator = GetComponent<Animator>();
    }

    /// <summary>
    /// Store an index into this treasure item, so that it can properly
    /// select itself. Also registers all of the treasure stats.
    /// </summary>
    public void Initialize(int assignedIdx)
    {
        _savedIdx = assignedIdx;
        _iconRenderer.sprite = TreasureData.TreasureIcon;
        switch (TreasureData.Type)
        {
            case TreasureType.CAT_PAW:
                LevelManager.Instance.OnPlayerAttack += () =>
                {
                    LevelManager.Instance.DealDamageToEnemy(1);
                };
                break;
        }
    }

    public void OnMouseOver()
    {
        TreasureSection.Instance.SelectTreasure(_savedIdx);
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
