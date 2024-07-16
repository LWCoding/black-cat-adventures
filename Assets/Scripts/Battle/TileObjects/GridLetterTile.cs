using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

[RequireComponent(typeof(Animator))]
public class GridLetterTile : LetterTile
{

    private Animator _animator;

    private void Awake()
    {
        _animator = GetComponent<Animator>();
        LevelManager.Instance.OnStateChanged += (newState) =>
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
        if (IsSelected || LevelManager.Instance.CurrentState is not PlayerTurnState) { return; }
        _animator.Play("StartHover");
    }

    private void OnMouseExit()
    {
        if (IsSelected || LevelManager.Instance.CurrentState is not PlayerTurnState) { return; }
        _animator.Play("StopHover");
    }

    /// <summary>
    /// When clicked, IF this tile is not selected and it is the
    /// player's turn to move, append this tile to the preview tiles
    /// and mark as selected.
    /// </summary>
    private void OnMouseDown()
    {
        if (IsSelected) return;
        if (LevelManager.Instance.CurrentState is not PlayerTurnState) return;
        WordPreview.Instance.AppendTile(_tile);
        OnMouseExit();
        IsSelected = true;  // Set this letter to selected
    }

}