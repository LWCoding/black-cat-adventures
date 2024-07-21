using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyHandler : CharacterHandler
{

    [Header("Enemy Properties")]
    [SerializeField] private GameObject _nextBattleObject;  // If null, this is the last enemy
    [SerializeField] private float _timeToNextObject = 1.3f;

    [Header("Optional Dialogue Assignments")]
    public List<DialogueInfo> DialogueToPlayOnMeet = new();

    [Header("Enemy Status")]
    public bool IsFightable = true;  // If false, then turn doesn't switch to player after meeting

    private void Start()
    {
        LevelManager.Instance.OnEnemyAttack += RenderAttack;
        if (_nextBattleObject != null)
        {
            // If there's a "next" object to render, transition to it
            HealthHandler.OnDeath += () =>
            {
                StartCoroutine(FadeAwayCoroutine());
                TransitionToNextEnemy();
                LevelManager.Instance.SetState(new WaitState());
            };
        } else
        {
            // Or else, the player has defeated all enemies in this level
            HealthHandler.OnDeath += () =>
            {
                StartCoroutine(FadeAwayCoroutine());
                LevelManager.Instance.SetState(new WinState());
            };
        }
    }

    protected override void RenderAttack()
    {
        if (HealthHandler.IsDead()) { return; }
        StartCoroutine(RenderAttackCoroutine(() =>
        {
            if (LevelManager.Instance.CurrentState is EnemyTurnState) {
                LevelManager.Instance.SetState(new PlayerTurnState());
            }
        }));
    }

    protected override IEnumerator RenderAttackCoroutine(Action codeToRunAfter)
    {
        Vector3 startingPos = transform.position;
        SetSprite(_charData.AttackSprite);
        for (int i = 0; i < 10; i++)
        {
            transform.position -= new Vector3(0.15f, 0, 0);
            yield return new WaitForSeconds(0.01f);
        }
        yield return new WaitForSeconds(0.1f);
        for (int i = 0; i < 10; i++)
        {
            transform.position += new Vector3(0.15f, 0, 0);
            yield return new WaitForSeconds(0.01f);
        }
        transform.position = startingPos;
        SetSprite(_charData.AliveSprite);
        yield return new WaitForSeconds(0.2f);

        codeToRunAfter.Invoke();
    }

    /// <summary>
    /// Animate towards the next enemy.
    /// </summary>
    public void TransitionToNextEnemy()
    {
        StartCoroutine(TransitionToNextEnemyCoroutine());
    }

    private IEnumerator TransitionToNextEnemyCoroutine()
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
            if (enemyHandler.IsFightable)
            {
                LevelManager.Instance.SetState(new PlayerTurnState());
            }
            LevelManager.Instance.SetNewEnemy(enemyHandler);
        }
        gameObject.SetActive(false);
    }

    /// <summary>
    /// Animate this character fading away into nothing.
    /// </summary>
    private IEnumerator FadeAwayCoroutine()
    {
        for (int i = 0; i < 25; i++)
        {
            _spriteRenderer.color -= new Color(0, 0, 0, 0.04f);
            if (HealthHandler.TextToUpdate != null)
            {
                HealthHandler.TextToUpdate.color -= new Color(0, 0, 0, 0.04f);
            }
            yield return new WaitForSeconds(0.06f);
        }
    }

}
