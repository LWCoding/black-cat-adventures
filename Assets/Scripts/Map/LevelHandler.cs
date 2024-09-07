using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;

[RequireComponent(typeof(Animator))]
[RequireComponent(typeof(PointerCursorOnHover))]
public class LevelHandler : MonoBehaviour
{

    [Header("Object Assignments")]
    [SerializeField] private SpriteRenderer _moundSprite;
    [SerializeField] private TextMeshPro _levelText;
    [SerializeField] private GameObject _lockObject;
    [Header("Level Properties")]
    public int LevelNumber = 0;
    public string LevelName;
    [Header("Level Assignments")]
    public LevelHandler PreviousLevel;
    public LevelHandler NextLevel;

    private bool _isCurrentLevel = false;   // True if this level is current selected level, else False
    
    public bool IsCurrentLevel
    {
        get => _isCurrentLevel;
        set
        {
            _isCurrentLevel = value;
            if (IsCurrentLevel)
            {
                // If this is now the current level, make prev visitable
                if (PreviousLevel != null)
                {
                    PreviousLevel.IsLevelVisitable = true;
                }
                // If we've completed the current level, allow access to next level
                if (NextLevel != null && GameManager.GameData.LevelsCompleted.Contains(LevelName))
                {
                    NextLevel.IsLevelVisitable = true;
                }
            } else 
            {
                // If this is no longer the current level, disable prev and next
                if (PreviousLevel != null)
                {
                    PreviousLevel.IsLevelVisitable = false;
                }
                if (NextLevel != null)
                {
                    NextLevel.IsLevelVisitable = false;
                }
            }
        }
    }

    private bool _isLevelVisitable = false;  // By default, you cannot visit any level
    public bool IsLevelVisitable
    {
        get => _isLevelVisitable;
        set
        {
            _pointerCursorOnHover.IsEnabled = value;  // Toggle cursor activation
            _isLevelVisitable = value;  // Set the actual value

            if (_isLevelVisitable)
            {
                _animator.Play(_isHovering ? "StartHover" : "StopHover");
            } else
            {
                _animator.Play("Disabled");
            }
        }
    }

    private Animator _animator;
    private PointerCursorOnHover _pointerCursorOnHover;
    private bool _isHovering = false;

    private void Awake()
    {
        _animator = GetComponent<Animator>();
        _pointerCursorOnHover = GetComponent<PointerCursorOnHover>();
        _animator.Play("Disabled");
        _levelText.text = LevelNumber.ToString();

        // If we haven't completed this level, nor the previous level,
        // then this level is locked
        if (!GameManager.GameData.LevelsCompleted.Contains(LevelName) && PreviousLevel != null && !GameManager.GameData.LevelsCompleted.Contains(PreviousLevel.LevelName))
        {
            _lockObject.SetActive(true);
        } else
        {
            _lockObject.SetActive(false);
        }
    }

    /// <summary>
    /// Play the selected level animation.
    /// </summary>
    public void SetAsSelectedLevel()
    {
        _animator.Play("Selected");
    }

    private void OnMouseEnter()
    {
        _isHovering = true;
        if (!_isLevelVisitable || IsCurrentLevel) { return; }
        _animator.Play("StartHover");
    }

    private void OnMouseExit()
    {
        _isHovering = false;
        if (!_isLevelVisitable || IsCurrentLevel) { return; } 
        _animator.Play("StopHover");
    }

    private void OnMouseDown()
    {
        if (IsCurrentLevel || !_isLevelVisitable) { return; }
        LevelsManager.Instance.SelectNewLevel(this);
    }

}
