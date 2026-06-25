using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

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
    public List<LevelHandler> PreviousLevels = new();
    public List<LevelHandler> NextLevels = new();

    private static readonly Color CompletedTint = new Color(0.4f, 0.85f, 0.45f);

    private bool _isCurrentLevel = false;

    public bool IsCurrentLevel
    {
        get => _isCurrentLevel;
        set
        {
            _isCurrentLevel = value;
            if (_isCurrentLevel)
            {
                _animator.Play("Selected");
            }
            else
            {
                // Return to the correct idle state when deselected.
                _animator.Play(_isLevelVisitable ? "StopHover" : "Disabled");
            }
        }
    }

    private bool _isLevelVisitable = false;
    public bool IsLevelVisitable => _isLevelVisitable;

    public void SetVisitable(bool visitable)
    {
        _isLevelVisitable = visitable;
        _pointerCursorOnHover.IsEnabled = visitable;

        if (_isLevelVisitable)
        {
            _animator.Play(_isHovering ? "StartHover" : "StopHover");
        }
        else if (!_isCurrentLevel)
        {
            _animator.Play("Disabled");
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
    /// Called by LevelsManager after all fields (SpaceNodeId, AuthoredSpace, PreviousLevels, etc.)
    /// have been set. Applies the initial label, lock, and completion state.
    /// </summary>
    public void Initialize()
    {
        // Determine label from space type. NodeLabel on the asset takes priority;
        // if absent, fall back to "?" for Unknown and "B" for any other authored space.
        if (AuthoredSpace == null)
        {
            _levelText.text = "";
        }
        else if (!string.IsNullOrEmpty(AuthoredSpace.NodeLabel))
        {
            _levelText.text = AuthoredSpace.NodeLabel;
        }
        else if (AuthoredSpace.SpaceTypeId == "Unknown")
        {
            _levelText.text = "?";
        }
        else
        {
            _levelText.text = "B";
        }

        bool completed = GameManager.GameData.CompletedNodeIds.Contains(SpaceNodeId);
        ApplyCompletedAppearance(completed);

        RefreshLockState();

        // Always start Disabled; LevelsManager will set the correct state on the
        // selected node and its visitable neighbours after all nodes are generated.
        _animator.Play("Disabled");
    }

    /// <summary>
    /// Re-evaluates the lock icon based on current completion data. A node is unlocked
    /// (no lock icon) as soon as it is completed or any parent is completed (travellable).
    /// Call this whenever CompletedNodeIds changes.
    /// </summary>
    public void RefreshLockState()
    {
        bool completed = GameManager.GameData.CompletedNodeIds.Contains(SpaceNodeId);
        bool reachable = PreviousLevels.Count == 0
            || PreviousLevels.Exists(p => GameManager.GameData.CompletedNodeIds.Contains(p.SpaceNodeId));
        _lockObject.SetActive(!completed && !reachable && PreviousLevels.Count > 0);
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

    /// <summary>
    /// Tints the mound sprite and hides the lock icon for completed nodes.
    /// </summary>
    public void ApplyCompletedAppearance(bool completed)
    {
        if (_moundSprite != null)
        {
            _moundSprite.color = completed ? CompletedTint : Color.white;
        }
        if (completed)
        {
            _lockObject.SetActive(false);
        }
    }

    public void SetAsSelectedLevel()
    {
        _animator.Play("Selected");
    }

    private void OnMouseEnter()
    {
        _isHovering = true;
        if (!_isLevelVisitable || _isCurrentLevel) { return; }
        _animator.Play("StartHover");
    }

    private void OnMouseExit()
    {
        _isHovering = false;
        if (!_isLevelVisitable || _isCurrentLevel) { return; }
        _animator.Play("StopHover");
    }

    private void OnMouseDown()
    {
        if (_isCurrentLevel || !_isLevelVisitable) { return; }
        LevelsManager.Instance.SelectNewLevel(this);
    }

}
