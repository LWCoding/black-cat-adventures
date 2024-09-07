using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LevelsManager : MonoBehaviour
{

    private static LevelsManager _instance;
    public static LevelsManager Instance
    {
        get
        {
            if (_instance == null)
            {
                _instance = FindAnyObjectByType<LevelsManager>();
            }
            return _instance;
        }
    }

    [Header("Object Assignments")]
    [SerializeField] private Transform _playerIconTransform;
    [SerializeField] private LevelHandler _firstLevel;

    [Header("Other Properties")]
    [SerializeField] private Vector3 _iconOffsetFromLevel;  // Offset between player icon and level

    private LevelHandler _currSelectedLevel;
    private List<LevelHandler> _allLevelHandlers;
    public string GetCurrentLevelString() => _currSelectedLevel.LevelName;  // Scene name of level selected

    public Action<int> OnLevelChanged = null;

    private void Awake()
    {
        _allLevelHandlers = new(FindObjectsOfType<LevelHandler>());
    }

    private void Start()
    {
        // Find current level, start at it
        foreach (LevelHandler lh in _allLevelHandlers)
        {
            if (lh.LevelName == GameManager.GameData.RecentLevelCompleted)
            {
                SelectNewLevel(lh);
                break;
            }
        }
        // Animate player to next level, since they've completed this
        if (_currSelectedLevel.NextLevel != null)
        {
            SelectNewLevel(_currSelectedLevel.NextLevel);
        }
    }

    /// <summary>
    /// Given a new LevelHandler, sets the current active level to
    /// that level.
    /// </summary>
    public void SelectNewLevel(LevelHandler newLevelHandler)
    {
        // If we don't have a selected level, this is the first level; skip animation
        if (_currSelectedLevel == null)
        {
            _currSelectedLevel = newLevelHandler;
            _currSelectedLevel.IsCurrentLevel = true;
            _currSelectedLevel.SetAsSelectedLevel();
            _playerIconTransform.position = newLevelHandler.transform.position + _iconOffsetFromLevel;
            OnLevelChanged.Invoke(newLevelHandler.LevelNumber);
            BattleButton.Instance.ToggleInteractability(true);
            return;
        }
        // Animate our icon to the new position, and then set the new selected level
        BattleButton.Instance.ToggleInteractability(false);
        StartCoroutine(AnimateIconToLevelCoroutine(newLevelHandler.transform.position + _iconOffsetFromLevel, () =>
        {
            _currSelectedLevel.IsCurrentLevel = false;  // If there was a "current" level, disable that
            _currSelectedLevel = newLevelHandler;
            _currSelectedLevel.IsCurrentLevel = true;
            _currSelectedLevel.SetAsSelectedLevel();
            OnLevelChanged.Invoke(newLevelHandler.LevelNumber);
            BattleButton.Instance.ToggleInteractability(true);
        }));
    }

    private IEnumerator AnimateIconToLevelCoroutine(Vector3 newPosition, Action codeToRunAfter)
    {
        Vector3 startPos = _playerIconTransform.position;
        Vector3 targetPos = newPosition;
        float currTime = 0;
        float timeToWait = 0.5f;
        while (currTime < timeToWait)
        {
            currTime += Time.deltaTime;
            _playerIconTransform.position = Vector3.Lerp(startPos, targetPos, currTime / timeToWait);
            yield return null;
        }
        codeToRunAfter.Invoke();
    }

#if UNITY_EDITOR
    /// <summary>
    /// Draw a line showing where the player icon would be placed
    /// above the first level for debugging purposes.
    /// </summary>
    public void OnDrawGizmos()
    {
        Gizmos.color = Color.green;
        Gizmos.DrawLine(_firstLevel.transform.position, _firstLevel.transform.position + _iconOffsetFromLevel);
    }
#endif

}
