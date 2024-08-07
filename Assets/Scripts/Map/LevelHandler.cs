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
    [Header("Level Properties")]
    public int LevelNumber = 0;
    public string LevelName;
    [Header("Level Assignments")]
    [SerializeField] private LevelHandler _prevLevel;
    [SerializeField] private LevelHandler _nextLevel;

    private bool _isCurrentLevel = false;   // True if this level is current selected level, else False
    
    public bool IsCurrentLevel
    {
        get => _isCurrentLevel;
        set
        {
            _isCurrentLevel = value;
            if (IsCurrentLevel)
            {
                // If this is now the current level, make prev and next visitable
                if (_prevLevel != null)
                {
                    _prevLevel.IsLevelVisitable = true;
                }
                if (_nextLevel != null)
                {
                    _nextLevel.IsLevelVisitable = true;
                }
            } else 
            {
                // If this is no longer the current level, disable prev and next
                if (_prevLevel != null)
                {
                    _prevLevel.IsLevelVisitable = false;
                }
                if (_nextLevel != null)
                {
                    _nextLevel.IsLevelVisitable = false;
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
        if (!IsCurrentLevel && !_isLevelVisitable) { return; }
        LevelsManager.Instance.SelectNewLevel(this);
    }

}
