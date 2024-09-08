using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

[RequireComponent(typeof(Animator))]
public class GridLetterTile : LetterTile
{

    [Header("Audio Assignments")]
    [SerializeField] private AudioClip _clickSFX;
    [SerializeField] private float _clickSFXVolume;

    private Animator _animator;

    private void Awake()
    {
        _animator = GetComponent<Animator>();
        BattleManager.Instance.OnStateChanged += (newState) =>
        {
            if (newState is not PlayerTurnState)
            {
                _animator.Play("StopHover");
                ToggleVisibility(false);
                IsVisibilityLocked = true;
            } else
            {
                IsVisibilityLocked = false;
                ToggleVisibility(true);
            }
        };
    }

    private void OnMouseEnter()
    {
        if (IsSelected || BattleManager.Instance.CurrentState is not PlayerTurnState) { return; }
        _animator.Play("StartHover");
    }

    private void OnMouseExit()
    {
        if (IsSelected || BattleManager.Instance.CurrentState is not PlayerTurnState) { return; }
        _animator.Play("StopHover");
    }

    private void OnMouseDown()
    {
        TrySelectTile();
    }

    /// <summary>
    /// IF this tile is not selected and it is the
    /// player's turn to move, append this tile to the preview tiles
    /// and mark as selected. 
    /// 
    /// Returns True if it worked, else False.
    /// </summary>
    public bool TrySelectTile()
    {
        if (IsSelected) return false;
        if (BattleManager.Instance.CurrentState is not PlayerTurnState) return false;
        AudioManager.Instance.PlayOneShot(_clickSFX, _clickSFXVolume);
        WordPreview.Instance.AppendTile(Tile);
        OnMouseExit();
        IsSelected = true;  // Set this letter to selected
        return true;
    }

}