using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

[RequireComponent(typeof(PointerCursorOnHover))]
public class ShuffleButton : MonoBehaviour
{

    private static ShuffleButton _instance;
    public static ShuffleButton Instance
    {
        get
        {
            if (_instance == null)
            {
                _instance = FindAnyObjectByType<ShuffleButton>();
            }
            return _instance;
        }
    }

    [Header("Object Assignments")]
    [SerializeField] private WordGrid _wordGrid;
    [SerializeField] private TextMeshPro _letterText;
    [SerializeField] private SpriteRenderer _bgRenderer;
    [Header("Audio Assignments")]
    [SerializeField] private AudioClip _shuffleSFX;

    private PointerCursorOnHover _pointerCursorOnHover;
    private bool _isInteractable = false;

    public static Action OnClickButton = null;

    private void Awake()
    {
        if (!ReferenceEquals(_instance, this))
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
        _pointerCursorOnHover = GetComponent<PointerCursorOnHover>();
    }

    private void Start()
    {
        // When starting, if it's player state, set interactable
        if (LevelManager.Instance.CurrentState is PlayerTurnState)
        {
            ToggleInteractability(true);
        }
        // If player turn, button is interactable, else no
        LevelManager.Instance.OnStateChanged += (state) =>
        {
            if (state is PlayerTurnState)
            {
                ToggleInteractability(true);
            }
            else
            {
                ToggleInteractability(false);
            }
        };
    }

    /// <summary>
    /// Toggle whether this tile is clickable or not.
    /// 
    /// isInteractable = true -> Button looks like it is interactable
    /// isInteractable = false -> Button looks like it isn't interactable
    /// </summary>
    private void ToggleInteractability(bool isInteractable)
    {
        _isInteractable = isInteractable;
        Color bgColor = _bgRenderer.color;
        Color letterColor = _letterText.color;
        _bgRenderer.color = new Color(bgColor.r, bgColor.g, bgColor.b, isInteractable ? 1 : 0.2f);
        _letterText.color = new Color(letterColor.r, letterColor.g, letterColor.b, isInteractable ? 1 : 0.3f);
        // Toggle cursor activation based on interactability
        _pointerCursorOnHover.IsEnabled = isInteractable;
    }

    private void OnMouseDown()
    {
        if (!_isInteractable) { return; }  // If not interactable, don't do anything
        if (LevelManager.Instance.CurrentState is not PlayerTurnState) { return; }
        OnClickButton?.Invoke();
        StartCoroutine(ShuffleGridCoroutine());
        AudioManager.Instance.PlayOneShot(_shuffleSFX);
    }

    /// <summary>
    /// Shift state to waiting, shuffle grid animation, and then shift to enemy turn.
    /// </summary>
    private IEnumerator ShuffleGridCoroutine()
    {
        // Make sure all tiles in the preview are erased
        WordPreview.Instance.EraseTiles();
        // Set state to wait state, and shuffle the board
        LevelManager.Instance.SetState(new WaitState());
        for (int i = 0; i < 5; i++)
        {
            _wordGrid.ShuffleBoard();
            yield return new WaitForSeconds(0.1f);
        }
        yield return new WaitForSeconds(1);
        // Shift turn to enemy
        LevelManager.Instance.SetState(new EnemyTurnState());
    }

}
