using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Random = UnityEngine.Random;

public class EnemyHandler : CharacterHandler
{

    [Header("Enemy Properties")]
    [SerializeField] private GameObject _nextBattleObject;  // If null, this is the last enemy
    [SerializeField] private float _timeToNextObject = 1.3f;

    [Header("Optional Dialogue Assignments")]
    public List<DialogueInfo> DialogueToPlayOnMeet = new();

    [Header("Game State Properties")]
    public bool ShouldStallBeforeTurn;  // If true, doesn't go straight to player turn

    public bool IsLastEnemy() => _nextBattleObject == null;

    private void Start()
    {
        BattleManager.Instance.OnEnemyAttack += RenderAttack;
        if (_nextBattleObject != null)
        {
            // If there's a "next" object to render, transition to it
            HealthHandler.OnDeath += () =>
            {
                StartCoroutine(FadeAwayCoroutine());
                TransitionToNextObject();
                BattleManager.Instance.SetState(new WaitState());
            };
        } else
        {
            // Or else, the player has defeated all enemies in this level
            HealthHandler.OnDeath += () =>
            {
                StartCoroutine(FadeAwayCoroutine());
                BattleManager.Instance.SetState(new WinState());
            };
        }
    }

    protected override void RenderAttack()
    {
        if (HealthHandler.IsDead()) { return; }
        StartCoroutine(RenderAttackCoroutine(() =>
        {
            if (BattleManager.Instance.CurrentState is EnemyTurnState) {
                BattleManager.Instance.SetState(new PlayerTurnState());
            }
        }));
    }

    protected override IEnumerator RenderAttackCoroutine(Action codeToRunAfter)
    {
        Vector3 startingPos = transform.position;
        List<EnemyAttack> possibleAttacks = ((EnemyData)CharData).Attacks;

        // If no possible attacks are available, return early
        if (possibleAttacks.Count == 0)
        {
            codeToRunAfter.Invoke();
            yield break;
        }

        EnemyAttack chosenAttack = possibleAttacks[Random.Range(0, possibleAttacks.Count)];

        // Flash the chosen attack in the enemy box
        if (EnemyInfoBox.Instance != null)
        {
            EnemyInfoBox.Instance.FlashAttackByName(chosenAttack.AttackName);
        }

        // Perform an animation depending on the type
        switch (chosenAttack.AnimType)
        {
            case AttackAnimation.DEFAULT:
                SetSprite(CharData.AttackSprite);
                float timeToWait = 0.1f;
                Vector3 targetPos = startingPos - new Vector3(1.5f, 0);
                while (timeToWait > 0)
                {
                    timeToWait -= Time.deltaTime;
                    transform.position = Vector3.Lerp(startingPos, targetPos, (0.1f - timeToWait) * 10);
                    yield return null;
                }
                BattleManager.Instance.RenderAttackAgainstPlayer(chosenAttack);
                yield return new WaitForSeconds(0.1f);
                timeToWait = 0.1f;
                while (timeToWait > 0)
                {
                    timeToWait -= Time.deltaTime;
                    transform.position = Vector3.Lerp(targetPos, startingPos, (0.1f - timeToWait) * 10);
                    yield return null;
                }
                SetSprite(CharData.AliveSprite);
                break;

            case AttackAnimation.SAY_NOTHING:
                SayDialogue(new DialogueInfo()
                {
                    Speaker = Faction.ENEMY,
                    Text = "...",
                    Duration = 1f
                });
                yield return new WaitForSeconds(1.2f);
                BattleManager.Instance.RenderAttackAgainstPlayer(chosenAttack);
                break;

            default:
                break;
        }

        transform.position = startingPos;
        yield return new WaitForSeconds(0.2f);

        codeToRunAfter.Invoke();
    }

    /// <summary>
    /// Animate towards the next object.
    /// </summary>
    public void TransitionToNextObject()
    {
        StartCoroutine(TransitionToNextObjectCoroutine());
    }

    private IEnumerator TransitionToNextObjectCoroutine()
    {
        _nextBattleObject.SetActive(true);
        yield return new WaitForSeconds(0.1f);
        float currTime = 0f;
        float timeToWait = _timeToNextObject;
        Vector3 startPos = _nextBattleObject.transform.position;
        while (currTime < timeToWait)
        {
            _nextBattleObject.transform.position = Vector3.Lerp(startPos, transform.position, currTime / timeToWait);
            currTime += Time.deltaTime; 
            yield return null;
        }
        yield return new WaitForSeconds(0.1f);
        // If this is an enemy object, register the enemy
        if (_nextBattleObject.TryGetComponent(out EnemyHandler enemyHandler))
        {
            // If the enemy is fightable, set the state to be the player's
            if (!enemyHandler.ShouldStallBeforeTurn && !enemyHandler.DialogueToPlayOnMeet[0].ShouldStallState)
            {
                BattleManager.Instance.SetState(new PlayerTurnState());
            }
            BattleManager.Instance.SetNewEnemy(enemyHandler);
            if(enemyHandler.IsLastEnemy())
        {
                BattleManager.Instance.OnReachedLastEnemy?.Invoke();
            }
        }
        gameObject.SetActive(false);
    }

    /// <summary>
    /// Animate this character fading away into nothing.
    /// </summary>
    private IEnumerator FadeAwayCoroutine()
    {
        StatusHandler.ToggleEffectVisibility(false);
        for (int i = 0; i < 25; i++)
        {
            _spriteRenderer.color -= new Color(0, 0, 0, 0.04f);
            if (HealthHandler.TextToUpdate != null)
            {
                HealthHandler.TextToUpdate.color -= new Color(0, 0, 0, 0.04f);
            }
            yield return new WaitForSeconds(0.05f);
        }
    }

}
