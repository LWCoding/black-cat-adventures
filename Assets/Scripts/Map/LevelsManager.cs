using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LevelsManager : Singleton<LevelsManager>
{

    [Header("Object Assignments")]
    [SerializeField] private Transform _playerIconTransform;
    [SerializeField] private BattleButton _battleButton;
    [SerializeField] private CameraFollow _cameraFollow;

    [Header("Node Generation")]
    [SerializeField] private LevelHandler _levelPrefab;
    [SerializeField] private Transform _levelsParent;
    [SerializeField] private int _nodeCount = 8;
    [SerializeField] private float _nodeSpacing = 4f;
    [SerializeField] private Vector3 _firstNodeLocalPos = new Vector3(-2f, -1.5f, 0f);

    [Header("Other Properties")]
    [SerializeField] private Vector3 _iconOffsetFromLevel;

    private LevelHandler _firstLevel;
    private LevelHandler _currSelectedLevel;
    private List<LevelHandler> _allLevelHandlers;
    public string GetCurrentLevelString() => _currSelectedLevel.LevelName;
    public string GetCurrentNodeId() => _currSelectedLevel?.SpaceNodeId;

    public Action<int> OnLevelChanged = null;

    protected override void Awake()
    {
        base.Awake();
        _allLevelHandlers = new();
        GenerateNodes();
    }

    private void GenerateNodes()
    {
        BattleSpaceData battleSpace = Resources.Load<BattleSpaceData>("ScriptableObjects/Spaces/BattleSpace");

        LevelHandler prev = null;
        for (int i = 0; i < _nodeCount; i++)
        {
            Vector3 localPos = _firstNodeLocalPos + Vector3.right * (_nodeSpacing * i);
            LevelHandler node = Instantiate(_levelPrefab, _levelsParent);
            node.gameObject.SetActive(false);

            node.SpaceNodeId = $"Node{i}";
            node.LevelNumber = i + 1;
            node.LevelName = $"Node{i}";
            node.AuthoredSpace = battleSpace;
            node.PreviousLevel = prev;
            if (prev != null)
            {
                prev.NextLevel = node;
            }

            node.transform.localPosition = localPos;
            node.gameObject.SetActive(true);
            node.Initialize();

            _allLevelHandlers.Add(node);
            if (i == 0) { _firstLevel = node; }
            prev = node;
        }

        if (_cameraFollow != null && _allLevelHandlers.Count > 0)
        {
            float firstX = _allLevelHandlers[0].transform.position.x;
            float lastX = _allLevelHandlers[^1].transform.position.x;
            _cameraFollow.SetXBounds(firstX, lastX);
        }
    }

    private void Start()
    {
        ResolveAndApplyAllSpaces();

        foreach (LevelHandler lh in _allLevelHandlers)
        {
            if (lh.SpaceNodeId == GameManager.GameData.RecentNodeEntered)
            {
                SelectNewLevel(lh);
                break;
            }
        }

        if (_currSelectedLevel == null)
        {
            SelectNewLevel(_firstLevel);
        }
        else if (_currSelectedLevel.NextLevel != null)
        {
            SelectNewLevel(_currSelectedLevel.NextLevel);
        }
    }

    /// <summary>
    /// For every node on the map: if it has an AuthoredSpace, resolve it (using the
    /// persisted seed so results are stable), store the result in GameData, and apply
    /// the resolved sprite to the node. Saves game state once if any new resolutions
    /// were generated.
    /// </summary>
    private void ResolveAndApplyAllSpaces()
    {
        if (GameManager.GameData.MapSeed == 0)
        {
            GameManager.GameData.MapSeed = UnityEngine.Random.Range(1, int.MaxValue);
        }
        System.Random rng = new(GameManager.GameData.MapSeed);

        bool anythingResolved = false;
        foreach (LevelHandler node in _allLevelHandlers)
        {
            if (node.AuthoredSpace == null) { continue; }

            ResolvedSpaceEntry existing = GameManager.GameData.GetResolvedSpace(node.SpaceNodeId);
            if (existing != null)
            {
                // Already resolved — just apply the saved appearance.
                ApplyAppearanceFromRegistry(node, existing);
                continue;
            }

            ResolvedSpace result = node.AuthoredSpace.Resolve(rng);
            ResolvedSpaceEntry entry = new()
            {
                SpaceNodeId = node.SpaceNodeId,
                ResolvedTypeId = result.ResolvedTypeId,
                SceneToLoad = result.SceneToLoad,
                PayloadId = result.PayloadId,
            };
            GameManager.GameData.SetResolvedSpace(entry);
            ApplyAppearanceFromRegistry(node, entry);
            anythingResolved = true;
        }

        if (anythingResolved)
        {
            SaveManager.SaveGame(GameManager.GameData);
        }
    }

    /// <summary>
    /// Applies the correct node sprite for a resolved space. For Unknown-turned-Battle
    /// we look up the concrete SpaceData so we can use its NodeSprite.
    /// The lookup is intentionally lightweight: we scan AuthoredSpace (which may be an
    /// UnknownSpaceData) and fall back to a Resources search by SpaceTypeId if needed.
    /// </summary>
    private void ApplyAppearanceFromRegistry(LevelHandler node, ResolvedSpaceEntry entry)
    {
        // If the authored space itself matches the resolved type, use it directly.
        if (node.AuthoredSpace != null && node.AuthoredSpace.SpaceTypeId == entry.ResolvedTypeId)
        {
            node.ApplyResolvedAppearance(node.AuthoredSpace.NodeSprite);
            return;
        }

        // Otherwise scan all SpaceData assets in Resources to find a matching type.
        SpaceData[] allSpaces = Resources.LoadAll<SpaceData>("ScriptableObjects/Spaces");
        foreach (SpaceData sd in allSpaces)
        {
            if (sd.SpaceTypeId == entry.ResolvedTypeId)
            {
                node.ApplyResolvedAppearance(sd.NodeSprite);
                return;
            }
        }
    }

    /// <summary>
    /// Returns the resolved entry for the currently selected node, or null if none.
    /// </summary>
    public ResolvedSpaceEntry GetCurrentResolvedSpace()
    {
        if (_currSelectedLevel == null) { return null; }
        return GameManager.GameData.GetResolvedSpace(_currSelectedLevel.SpaceNodeId);
    }

    public void SelectNewLevel(LevelHandler newLevelHandler)
    {
        if (_currSelectedLevel == null)
        {
            _currSelectedLevel = newLevelHandler;
            _currSelectedLevel.IsCurrentLevel = true;
            _currSelectedLevel.SetAsSelectedLevel();
            _playerIconTransform.position = newLevelHandler.transform.position + _iconOffsetFromLevel;
            OnLevelChanged.Invoke(newLevelHandler.LevelNumber);
            _battleButton.ToggleInteractability(true);
            return;
        }
        _battleButton.ToggleInteractability(false);
        StartCoroutine(AnimateIconToLevelCoroutine(newLevelHandler.transform.position + _iconOffsetFromLevel, () =>
        {
            _currSelectedLevel.IsCurrentLevel = false;
            _currSelectedLevel = newLevelHandler;
            _currSelectedLevel.IsCurrentLevel = true;
            _currSelectedLevel.SetAsSelectedLevel();
            OnLevelChanged.Invoke(newLevelHandler.LevelNumber);
            _battleButton.ToggleInteractability(true);
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
    public void OnDrawGizmos()
    {
        if (_firstLevel == null) { return; }
        Gizmos.color = Color.green;
        Gizmos.DrawLine(_firstLevel.transform.position, _firstLevel.transform.position + _iconOffsetFromLevel);
    }
#endif

}
