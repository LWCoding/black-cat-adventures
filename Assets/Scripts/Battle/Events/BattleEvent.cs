using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using UnityEngine;

/// <summary>
/// Generic EventBehaviour for battle-style events that run in the Level scene.
/// Reads its full configuration from a BattleEventData asset — which enemies
/// to spawn, any start statuses, an optional turn-limit flee, and the reward
/// granted when the last enemy is defeated. No subclass is needed per event;
/// author a new BattleEventData asset and register it in _EventDatabase.
///
/// LevelSpawner.AwakeEventMode detects INeedsEventSceneContext and supplies the
/// scene prefab/anchor references before calling BeginEvent.
/// </summary>
public class BattleEvent : EventBehaviour, INeedsEventSceneContext
{
    private EventSceneContext _ctx;
    private EnemyHandler      _lastEnemy;
    private int               _turnsLeft;
    private bool              _eventEnded = false;

    public void SetSceneContext(EventSceneContext ctx) => _ctx = ctx;

    public override void BeginEvent()
    {
        BattleEventData data = (BattleEventData)Data;

        if (data.Encounter == null || data.Encounter.Enemies.Count == 0)
        {
            Debug.LogError($"[BattleEvent] '{data.EventId}' has no Encounter assigned.");
            ReturnToMap();
            return;
        }

        if (_ctx.EnemyPrefab == null)
        {
            Debug.LogError($"[BattleEvent] '{data.EventId}' received no EnemyPrefab from LevelSpawner.");
            ReturnToMap();
            return;
        }

        List<EnemyHandler> spawned = SpawnEnemyChain(data.Encounter);

        _lastEnemy = spawned[^1];
        _lastEnemy.SuppressDefaultDeath = true;
        _lastEnemy.HealthHandler.OnDeath += OnLastEnemyDefeated;

        ApplyStartStatuses(data.StartStatuses, spawned);

        if (data.TurnLimit > 0)
        {
            _turnsLeft = data.TurnLimit;
            BattleManager.Instance.OnStateChanged += OnBattleStateChanged;
        }

        // Hand off to BattleManager — its Start() will call SetNewEnemy(CurrEnemyHandler)
        // and kick off the first PlayerTurnState automatically.
        BattleManager.Instance.CurrEnemyHandler = spawned[0];
    }

    // ─── Enemy chain ─────────────────────────────────────────────────────────

    private List<EnemyHandler> SpawnEnemyChain(Encounter encounter)
    {
        List<EnemyHandler> spawned = new();
        for (int i = 0; i < encounter.Enemies.Count; i++)
        {
            EnemySpawn spawn = encounter.Enemies[i];
            Vector3 pos = (i == 0 ? _ctx.EnemySpawnAnchor.position : _ctx.StagingAnchor.position)
                        + (Vector3)spawn.SpawnOffset;

            GameObject obj = Instantiate(_ctx.EnemyPrefab, pos, Quaternion.identity, _ctx.SpawnedObjectsParent);
            EnemyHandler eh = obj.GetComponent<EnemyHandler>();
            eh.SetCharacterData(spawn.EnemyData);
            // Copy list so BattleManager's RemoveAt calls don't mutate the Encounter asset.
            eh.DialogueToPlayOnMeet  = new List<DialogueInfo>(spawn.DialogueToPlayOnMeet);
            eh.ShouldStallBeforeTurn = spawn.ShouldStallBeforeTurn;
            eh.SetTimeToNextObject(spawn.TimeToNextObject);
            if (i > 0) { obj.SetActive(false); }
            spawned.Add(eh);
        }

        for (int i = 0; i < spawned.Count - 1; i++)
        {
            spawned[i].SetNextBattleObject(spawned[i + 1].gameObject);
        }

        return spawned;
    }

    private static void ApplyStartStatuses(List<StartStatus> statuses, List<EnemyHandler> spawned)
    {
        foreach (StartStatus ss in statuses)
        {
            if (ss.Status == null) { continue; }
            if (ss.EnemyIndex < 0 || ss.EnemyIndex >= spawned.Count) { continue; }
            spawned[ss.EnemyIndex].StatusHandler.GainStatusEffect(ss.Status, ss.Amplifier);
        }
    }

    // ─── Turn-limit (flee) ───────────────────────────────────────────────────

    private void OnBattleStateChanged(State state)
    {
        if (_eventEnded) { return; }
        if (state is not EnemyTurnState) { return; }

        _turnsLeft--;
        if (_turnsLeft <= 0 && !_lastEnemy.HealthHandler.IsDead())
        {
            _eventEnded = true;
            BattleManager.Instance.OnStateChanged -= OnBattleStateChanged;
            _lastEnemy.HealthHandler.OnDeath = null;
            StartCoroutine(FleeCoroutine());
        }
    }

    // ─── Last-enemy defeat ───────────────────────────────────────────────────

    private void OnLastEnemyDefeated()
    {
        if (_eventEnded) { return; }
        _eventEnded = true;

        if (((BattleEventData)Data).TurnLimit > 0)
        {
            BattleManager.Instance.OnStateChanged -= OnBattleStateChanged;
        }

        BattleManager.Instance.SetState(new WaitState());
        EnemyInfoBox.Instance?.ClearInfo();

        StartCoroutine(RevealRewardCoroutine());
    }

    private IEnumerator RevealRewardCoroutine()
    {
        // Brief pause so the dead sprite is visible before the reward appears.
        yield return new WaitForSeconds(0.7f);

        BattleEventData data = (BattleEventData)Data;
        switch (data.WinReward.Type)
        {
            case OutcomeType.ChooseTreasure:
                yield return StartCoroutine(RunChooseTreasure(data.WinReward.ChooseFrom));
                break;

            case OutcomeType.GrantRandomTreasures:
                EventOutcomes.GrantRandom(data.WinReward.Count);
                yield return new WaitForSeconds(0.5f);
                ReturnToMap();
                break;

            default:
                ReturnToMap();
                break;
        }
    }

    private IEnumerator RunChooseTreasure(int chooseFrom)
    {
        List<Treasure> choices = GameManager.GameData.GetRandomUnownedTreasures(chooseFrom);
        if (choices.Count == 0)
        {
            ReturnToMap();
            yield break;
        }

        bool picked = false;
        TreasureChoiceScreen.Instance.Show(choices, chosen =>
        {
            picked = true;
            StartCoroutine(FlyTreasureToPlayerCoroutine(chosen));
        });

        yield return new WaitUntil(() => picked);
    }

    /// <summary>
    /// Animates the chosen treasure icon flying from the last enemy's position
    /// to the player, grants it, then returns to the map — matching the feel
    /// of opening a regular TreasureChest.
    /// </summary>
    private IEnumerator FlyTreasureToPlayerCoroutine(Treasure chosen)
    {
        SpriteRenderer boxRenderer = _lastEnemy != null
            ? _lastEnemy.GetComponentInChildren<SpriteRenderer>()
            : null;
        Vector3 startPos = boxRenderer != null
            ? boxRenderer.transform.position
            : (_lastEnemy != null ? _lastEnemy.transform.position : Vector3.zero);

        GameObject iconObj = new("TreasureFlyIcon");
        iconObj.transform.position   = startPos;
        iconObj.transform.localScale = new Vector3(0.8f, 0.8f, 1f);

        SpriteRenderer icon = iconObj.AddComponent<SpriteRenderer>();
        icon.sprite = chosen.TreasureIcon;
        icon.color  = Color.white;
        if (boxRenderer != null)
        {
            icon.sortingLayerID = boxRenderer.sortingLayerID;
            icon.sortingOrder   = boxRenderer.sortingOrder + 1;
        }

        yield return new WaitForSeconds(0.2f);

        Vector3 targetPos = BattleManager.Instance.PlayerHandler.transform.position;
        Sequence seq = DOTween.Sequence();
        seq.Append(iconObj.transform.DOMove(targetPos, 0.7f).SetEase(Ease.InOutQuad));
        seq.Join(icon.DOFade(0f, 0.7f));
        yield return seq.WaitForCompletion();

        GameManager.GameData.UnlockedTreasures.Add(chosen);
        Destroy(iconObj);

        yield return new WaitForSeconds(0.4f);
        ReturnToMap();
    }

    // ─── Flee ────────────────────────────────────────────────────────────────

    private IEnumerator FleeCoroutine()
    {
        yield return null;  // let EnemyTurnState finish its frame

        BattleManager.Instance.SetState(new WaitState());
        EnemyInfoBox.Instance?.ClearInfo();

        if (_ctx.StagingAnchor != null)
        {
            bool done = false;
            _lastEnemy.transform
                .DOMove(_ctx.StagingAnchor.position, 0.8f)
                .SetEase(Ease.InBack)
                .OnComplete(() => done = true);
            yield return new WaitUntil(() => done);
        }

        yield return new WaitForSeconds(0.4f);
        ReturnToMap();
    }

    // ─── Map return ──────────────────────────────────────────────────────────

    private void ReturnToMap() => EventFlow.CompleteAndReturnToMap();
}
