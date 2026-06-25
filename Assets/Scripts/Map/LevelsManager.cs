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
    [SerializeField] private Transform _edgesParent;

    [Header("Graph Generation")]
    [SerializeField] private LevelHandler _levelPrefab;
    [SerializeField] private Transform _levelsParent;
    [SerializeField] private int _columnCount = 7;
    [SerializeField] private int _minNodesPerColumn = 2;
    [SerializeField] private int _maxNodesPerColumn = 3;
    [SerializeField] private float _columnSpacing = 4f;
    [SerializeField] private float _rowSpacing = 2.5f;
    [SerializeField] private Vector3 _graphOrigin = new Vector3(-12f, -1.5f, 0f);

    [Header("Edge Appearance")]
    [SerializeField] private float _edgeWidth = 0.12f;
    [SerializeField] private Color _edgeColor = new Color(0.55f, 0.45f, 0.35f, 1f);
    [Range(0f, 1f)]
    [SerializeField] private float _extraEdgeChance = 0.6f;

    [Header("Other Properties")]
    [SerializeField] private Vector3 _iconOffsetFromLevel;
    [Header("Node Type Weights")]
    [Tooltip("Relative weight for battle nodes on middle columns.")]
    [SerializeField] private float _battleWeight = 1f;
    [Tooltip("Relative weight for unknown (?) nodes on middle columns.")]
    [SerializeField] private float _unknownWeight = 1f;
    [Tooltip("Relative weight for miniboss nodes on middle columns.")]
    [SerializeField] private float _minibossWeight = 0.2f;

    private LevelHandler _startLevel;
    private LevelHandler _currSelectedLevel;
    private LevelHandler _frontierLevel;
    private List<LevelHandler> _allLevelHandlers = new();

    // Nested list: _columns[col][row] = LevelHandler
    private List<List<LevelHandler>> _columns = new();

    public string GetCurrentLevelString() => _currSelectedLevel.LevelName;
    public string GetCurrentNodeId() => _currSelectedLevel?.SpaceNodeId;
    public LevelHandler CurrentLevel => _currSelectedLevel;

    public Action<LevelHandler> OnLevelChanged = null;

    protected override void Awake()
    {
        base.Awake();

        // Seed must exist before GenerateNodes so topology is deterministic.
        if (GameManager.GameData.MapSeed == 0)
        {
            GameManager.GameData.MapSeed = UnityEngine.Random.Range(1, int.MaxValue);
        }

        GenerateNodes();
    }

    // ─── Graph Generation ────────────────────────────────────────────────────

    private void GenerateNodes()
    {
        // Two independent RNG streams derived from the same seed so topology and
        // space authoring are both stable but don't interfere.
        System.Random layoutRng = new(GameManager.GameData.MapSeed);
        System.Random spaceRng  = new(GameManager.GameData.MapSeed ^ 0x5A5A5A5A);

        BattleSpaceData  battleSpace   = Resources.Load<BattleSpaceData> ("ScriptableObjects/Spaces/BattleSpace");
        UnknownSpaceData unknownSpace  = Resources.Load<UnknownSpaceData>("ScriptableObjects/Spaces/UnknownSpace");
        MinibossSpaceData minibossSpace = Resources.Load<MinibossSpaceData>("ScriptableObjects/Spaces/MinibossSpace");

        // Build node grid ---------------------------------------------------
        for (int col = 0; col < _columnCount; col++)
        {
            int nodeCount;
            if (col == 0 || col == _columnCount - 1)
            {
                nodeCount = 1; // Start and End are single nodes.
            }
            else
            {
                nodeCount = layoutRng.Next(_minNodesPerColumn, _maxNodesPerColumn + 1);
            }

            float totalHeight = (nodeCount - 1) * _rowSpacing;
            List<LevelHandler> column = new();

            for (int row = 0; row < nodeCount; row++)
            {
                float x = _graphOrigin.x + col * _columnSpacing;
                float y = _graphOrigin.y - totalHeight * 0.5f + row * _rowSpacing;
                Vector3 localPos = new Vector3(x, y, _graphOrigin.z);

                LevelHandler node = Instantiate(_levelPrefab, _levelsParent);
                node.gameObject.SetActive(false);

                node.SpaceNodeId = $"Node_{col}_{row}";
                node.LevelNumber  = col;
                node.LevelName    = $"Node_{col}_{row}";

                // Author space per column:
                // col 0 = Start (null space = tutorial marker)
                // col last = End (always Battle)
                // middle = weighted random Battle, Unknown, or Miniboss
                if (col == 0)
                {
                    node.AuthoredSpace = null;
                }
                else if (col == _columnCount - 1)
                {
                    node.AuthoredSpace = battleSpace;
                }
                else
                {
                    node.AuthoredSpace = RollAuthoredSpace(
                        spaceRng, battleSpace, unknownSpace, minibossSpace);
                }

                node.transform.localPosition = localPos;
                node.gameObject.SetActive(true);

                column.Add(node);
                _allLevelHandlers.Add(node);
            }

            _columns.Add(column);
            if (col == 0) { _startLevel = column[0]; }
        }

        // Build edges -------------------------------------------------------
        for (int col = 0; col < _columnCount - 1; col++)
        {
            List<LevelHandler> colA = _columns[col];
            List<LevelHandler> colB = _columns[col + 1];
            BuildEdges(colA, colB, layoutRng);
        }

        // Initialize each node now that links are set -----------------------
        foreach (LevelHandler node in _allLevelHandlers)
        {
            node.Initialize();
        }

        // Camera bounds from Start to End.
        if (_cameraFollow != null && _allLevelHandlers.Count > 0)
        {
            float firstX = _columns[0][0].transform.position.x;
            float lastX  = _columns[_columnCount - 1][0].transform.position.x;
            _cameraFollow.SetXBounds(firstX, lastX);
        }

        // Spawn visual edges.
        foreach (LevelHandler node in _allLevelHandlers)
        {
            foreach (LevelHandler child in node.NextLevels)
            {
                SpawnEdge(node.transform.position, child.transform.position);
            }
        }
    }

    /// <summary>
    /// Picks a middle-column node type using configurable relative weights.
    /// Miniboss is authored directly (not via Unknown) so "?" never resolves to it.
    /// </summary>
    private SpaceData RollAuthoredSpace(
        System.Random rng,
        BattleSpaceData battle,
        UnknownSpaceData unknown,
        MinibossSpaceData miniboss)
    {
        float minibossW = miniboss != null ? _minibossWeight : 0f;
        float total = _battleWeight + _unknownWeight + minibossW;
        if (total <= 0f) { return battle; }

        float roll = (float)(rng.NextDouble() * total);
        if (roll < _battleWeight) { return battle; }
        roll -= _battleWeight;
        if (roll < _unknownWeight) { return unknown; }
        return miniboss != null ? miniboss : battle;
    }

    /// <summary>
    /// Wires edges between two adjacent columns with a guaranteed full path plus
    /// probabilistic extra edges that are not biased toward any particular row.
    /// </summary>
    private void BuildEdges(List<LevelHandler> colA, List<LevelHandler> colB, System.Random rng)
    {
        // Step 1: each A node -> its nearest B node (every A gets at least one child).
        foreach (LevelHandler a in colA)
        {
            LevelHandler nearest = NearestNode(a, colB);
            AddEdge(a, nearest);
        }

        // Step 2: any parentless B node -> its nearest A node.
        foreach (LevelHandler b in colB)
        {
            if (b.PreviousLevels.Count == 0)
            {
                LevelHandler nearest = NearestNode(b, colA);
                AddEdge(nearest, b);
            }
        }

        // Step 3: probabilistic extra edges.
        // Enumerate all non-crossing A->B pairs not yet connected, shuffle them so
        // no positional bias, then add each with _extraEdgeChance probability.
        List<(LevelHandler a, LevelHandler b)> candidates = new();
        for (int ai = 0; ai < colA.Count; ai++)
        {
            for (int bi = 0; bi < colB.Count; bi++)
            {
                LevelHandler a = colA[ai];
                LevelHandler b = colB[bi];
                if (a.NextLevels.Contains(b)) { continue; }

                // Non-crossing check: an edge (ai, bi) crosses another (aj, bj) if
                // ai < aj but bi > bj (or vice versa). We skip if this edge would
                // cross any existing edge.
                bool crosses = false;
                for (int aj = 0; aj < colA.Count && !crosses; aj++)
                {
                    foreach (LevelHandler bj in colA[aj].NextLevels)
                    {
                        int bj_idx = colB.IndexOf(bj);
                        if (bj_idx < 0) { continue; }
                        if ((ai < aj && bi > bj_idx) || (ai > aj && bi < bj_idx))
                        {
                            crosses = true;
                            break;
                        }
                    }
                }
                if (!crosses)
                {
                    candidates.Add((a, b));
                }
            }
        }

        // Shuffle candidates to avoid positional bias.
        for (int i = candidates.Count - 1; i > 0; i--)
        {
            int j = rng.Next(i + 1);
            (candidates[i], candidates[j]) = (candidates[j], candidates[i]);
        }

        foreach ((LevelHandler a, LevelHandler b) in candidates)
        {
            if (rng.NextDouble() < _extraEdgeChance)
            {
                AddEdge(a, b);
            }
        }
    }

    private static void AddEdge(LevelHandler parent, LevelHandler child)
    {
        if (!parent.NextLevels.Contains(child))
        {
            parent.NextLevels.Add(child);
        }
        if (!child.PreviousLevels.Contains(parent))
        {
            child.PreviousLevels.Add(parent);
        }
    }

    private static LevelHandler NearestNode(LevelHandler from, List<LevelHandler> candidates)
    {
        LevelHandler nearest  = candidates[0];
        float        minDist  = Mathf.Abs(from.transform.localPosition.y - candidates[0].transform.localPosition.y);
        for (int i = 1; i < candidates.Count; i++)
        {
            float d = Mathf.Abs(from.transform.localPosition.y - candidates[i].transform.localPosition.y);
            if (d < minDist)
            {
                minDist  = d;
                nearest  = candidates[i];
            }
        }
        return nearest;
    }

    private void SpawnEdge(Vector3 from, Vector3 to)
    {
        if (_edgesParent == null) { return; }
        GameObject go = new("Edge");
        go.transform.SetParent(_edgesParent, false);

        LineRenderer lr = go.AddComponent<LineRenderer>();
        lr.material          = new Material(Shader.Find("Sprites/Default"));
        lr.startColor        = _edgeColor;
        lr.endColor          = _edgeColor;
        lr.startWidth        = _edgeWidth;
        lr.endWidth          = _edgeWidth;
        lr.positionCount     = 2;
        lr.useWorldSpace     = true;
        lr.sortingOrder      = -1;
        lr.SetPosition(0, new Vector3(from.x, from.y, 0f));
        lr.SetPosition(1, new Vector3(to.x,   to.y,   0f));
    }

    // ─── Start / Selection Flow ──────────────────────────────────────────────

    private void Start()
    {
        // Auto-complete the Start (tutorial) node.
        if (!GameManager.GameData.CompletedNodeIds.Contains(_startLevel.SpaceNodeId))
        {
            GameManager.GameData.CompletedNodeIds.Add(_startLevel.SpaceNodeId);
            SaveManager.SaveGame(GameManager.GameData);
        }

        // Refresh start node's completed appearance now that it's in CompletedNodeIds.
        _startLevel.ApplyCompletedAppearance(true);
        _startLevel.Initialize();

        ResolveAndApplyAllSpaces();

        // Now that the start node is completed, re-evaluate every node's lock icon so
        // its children (which are now reachable) shed their stale locked appearance.
        foreach (LevelHandler node in _allLevelHandlers)
        {
            node.RefreshLockState();
        }

        // Find the node the player most recently entered (if any).
        LevelHandler entered = _allLevelHandlers.Find(
            lh => lh.SpaceNodeId == GameManager.GameData.RecentNodeEntered);

        // The frontier is the deepest completed node the player branches forward from.
        LevelHandler frontier;
        if (entered != null && IsCompleted(entered))
        {
            frontier = entered;                                              // just beaten a node
        }
        else if (entered != null)
        {
            frontier = entered.PreviousLevels.Find(IsCompleted) ?? _startLevel; // returned mid-run
        }
        else
        {
            frontier = _startLevel;                                          // fresh game
        }

        EstablishFrontier(frontier);

        // Start on the node the player was on if it's still an unplayed frontier child;
        // Start on the attempted child if it wasn't completed (loss/retry case);
        // otherwise stay on the frontier so the player chooses where to go next.
        LevelHandler initial =
            (entered != null && !IsCompleted(entered) && frontier.NextLevels.Contains(entered))
            ? entered
            : frontier;

        SelectNewLevel(initial);
    }

    // ─── Space Resolution ─────────────────────────────────────────────────────

    /// <summary>
    /// For every node on the map: resolve its space using the persisted seed,
    /// store the result in GameData, and apply the resolved sprite. Saves once
    /// if any new resolutions were generated.
    /// </summary>
    private void ResolveAndApplyAllSpaces()
    {
        System.Random rng = new(GameManager.GameData.MapSeed);

        bool anythingResolved = false;
        foreach (LevelHandler node in _allLevelHandlers)
        {
            if (node.AuthoredSpace == null) { continue; }

            ResolvedSpaceEntry existing = GameManager.GameData.GetResolvedSpace(node.SpaceNodeId);
            if (existing != null)
            {
                ApplyAppearanceFromRegistry(node, existing);
                continue;
            }

            ResolvedSpace result = node.AuthoredSpace.Resolve(rng);
            ResolvedSpaceEntry entry = new()
            {
                SpaceNodeId    = node.SpaceNodeId,
                ResolvedTypeId = result.ResolvedTypeId,
                SceneToLoad    = result.SceneToLoad,
                PayloadId      = result.PayloadId,
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

    private void ApplyAppearanceFromRegistry(LevelHandler node, ResolvedSpaceEntry entry)
    {
        if (node.AuthoredSpace != null && node.AuthoredSpace.SpaceTypeId == entry.ResolvedTypeId)
        {
            node.ApplyResolvedAppearance(node.AuthoredSpace.NodeSprite);
            return;
        }

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

    // ─── Selection ───────────────────────────────────────────────────────────

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
        if (newLevelHandler == null) { return; }

        // Deselect the previous node — its animator state is handled by the
        // IsCurrentLevel setter (returns to StopHover if still visitable, else Disabled).
        if (_currSelectedLevel != null)
        {
            _currSelectedLevel.IsCurrentLevel = false;
        }

        if (_currSelectedLevel == null)
        {
            _currSelectedLevel = newLevelHandler;
            _currSelectedLevel.IsCurrentLevel = true;
            _playerIconTransform.position = newLevelHandler.transform.position + _iconOffsetFromLevel;
            OnLevelChanged?.Invoke(newLevelHandler);
            _battleButton.ToggleInteractability(!IsCompleted(newLevelHandler));
            return;
        }

        _battleButton.ToggleInteractability(false);
        List<Vector3> path = BuildIconPath(_currSelectedLevel, newLevelHandler);
        StartCoroutine(AnimateIconAlongPathCoroutine(path, () =>
        {
            _currSelectedLevel = newLevelHandler;
            _currSelectedLevel.IsCurrentLevel = true;
            OnLevelChanged?.Invoke(newLevelHandler);
            _battleButton.ToggleInteractability(!IsCompleted(newLevelHandler));
        }));
    }

    // ─── Frontier helpers ────────────────────────────────────────────────────

    private static bool IsCompleted(LevelHandler n) =>
        GameManager.GameData.CompletedNodeIds.Contains(n.SpaceNodeId);

    /// <summary>
    /// Sets the frontier (deepest completed node) and marks only its non-completed
    /// children as visitable. Completed and unreachable nodes are never made visitable,
    /// so they remain Disabled and unclickable.
    /// </summary>
    private void EstablishFrontier(LevelHandler frontier)
    {
        _frontierLevel = frontier;
        frontier.SetVisitable(true);                 // allow temporary backtrack to the just-completed node
        foreach (LevelHandler child in frontier.NextLevels)
        {
            child.SetVisitable(!IsCompleted(child));
        }
    }

    /// <summary>
    /// Builds the waypoint list for the icon to travel from <paramref name="from"/> to
    /// <paramref name="to"/>. Direct edges (parent↔child) are a single waypoint. Moves
    /// between nodes that share a parent but have no direct edge route through that shared
    /// parent so the icon always follows an existing graph edge.
    /// </summary>
    private List<Vector3> BuildIconPath(LevelHandler from, LevelHandler to)
    {
        List<Vector3> waypoints = new();

        bool directlyConnected = from.NextLevels.Contains(to) || from.PreviousLevels.Contains(to);
        if (!directlyConnected)
        {
            // Prefer _frontierLevel if it is the shared parent; otherwise find any common parent.
            LevelHandler via =
                (_frontierLevel != null
                 && from.PreviousLevels.Contains(_frontierLevel)
                 && to.PreviousLevels.Contains(_frontierLevel))
                ? _frontierLevel
                : from.PreviousLevels.Find(p => to.PreviousLevels.Contains(p));

            if (via != null)
            {
                waypoints.Add(via.transform.position + _iconOffsetFromLevel);
            }
        }

        waypoints.Add(to.transform.position + _iconOffsetFromLevel);
        return waypoints;
    }

    private IEnumerator AnimateIconAlongPathCoroutine(List<Vector3> waypoints, Action codeToRunAfter)
    {
        const float totalTime = 0.5f;
        float segmentTime = waypoints.Count > 0 ? totalTime / waypoints.Count : totalTime;
        foreach (Vector3 target in waypoints)
        {
            Vector3 startPos  = _playerIconTransform.position;
            float   currTime  = 0f;
            while (currTime < segmentTime)
            {
                currTime += Time.deltaTime;
                _playerIconTransform.position = Vector3.Lerp(startPos, target, currTime / segmentTime);
                yield return null;
            }
            _playerIconTransform.position = target;
        }
        codeToRunAfter?.Invoke();
    }

#if UNITY_EDITOR
    public void OnDrawGizmos()
    {
        if (_startLevel == null) { return; }
        Gizmos.color = Color.green;
        Gizmos.DrawLine(_startLevel.transform.position, _startLevel.transform.position + _iconOffsetFromLevel);
    }
#endif

}
