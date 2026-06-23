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
    [Header("Space Properties")]
    [Tooltip("Stable unique id for this node (used as a key in GameData.ResolvedSpaces).")]
    public string SpaceNodeId;
    [Tooltip("The SpaceData asset authored for this node (e.g. BattleSpaceData, UnknownSpaceData).")]
    public SpaceData AuthoredSpace;
    [Header("Level Properties")]
    public int LevelNumber = 0;
    public string LevelName;
    [Header("Level Assignments")]
    public LevelHandler PreviousLevel;
    public LevelHandler NextLevel;

    private bool _isCurrentLevel = false;
    
    public bool IsCurrentLevel
    {
        get => _isCurrentLevel;
        set
        {
            _isCurrentLevel = value;
            if (IsCurrentLevel)
            {
                if (PreviousLevel != null)
                {
                    PreviousLevel.IsLevelVisitable = true;
                }
                if (NextLevel != null && GameManager.GameData.CompletedNodeIds.Contains(SpaceNodeId))
                {
                    NextLevel.IsLevelVisitable = true;
                }
            } else 
            {
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

    private bool _isLevelVisitable = false;
    public bool IsLevelVisitable
    {
        get => _isLevelVisitable;
        set
        {
            _pointerCursorOnHover.IsEnabled = value;
            _isLevelVisitable = value;

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
    }

    /// <summary>
    /// Called by LevelsManager after all fields (SpaceNodeId, LevelNumber, PreviousLevel, etc.)
    /// have been set. Applies the initial lock/text display state.
    /// </summary>
    public void Initialize()
    {
        _levelText.text = LevelNumber.ToString();

        bool prevCompleted = PreviousLevel == null || GameManager.GameData.CompletedNodeIds.Contains(PreviousLevel.SpaceNodeId);
        bool thisCompleted = GameManager.GameData.CompletedNodeIds.Contains(SpaceNodeId);
        _lockObject.SetActive(!thisCompleted && !prevCompleted && PreviousLevel != null);
    }

    /// <summary>
    /// Apply the resolved space's node sprite so the node visually reflects its type.
    /// Called by LevelsManager after Unknown spaces are resolved.
    /// </summary>
    public void ApplyResolvedAppearance(Sprite nodeSprite)
    {
        if (nodeSprite != null && _moundSprite != null)
        {
            _moundSprite.sprite = nodeSprite;
        }
    }

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
