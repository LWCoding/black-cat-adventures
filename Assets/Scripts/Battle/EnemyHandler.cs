using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyHandler : CharacterHandler
{

    [Header("Enemy Properties")]
    [SerializeField] private GameObject _nextEnemyObject;  // If null, this is the last enemy

    private const float TIME_TO_ANIMATE_NEXT_ENEMY_IN = 1.4f;  // Animation time between enemies

    private void Start()
    {
        LevelManager.Instance.OnEnemyAttack += RenderAttack;
        if (_nextEnemyObject != null)
        {
            // If there's a "next" enemy to render, transition to it
            _healthHandler.OnDeath += () =>
            {
                StartCoroutine(FadeAwayCoroutine());
                TransitionToNextEnemy();
                LevelManager.Instance.SetState(new WaitState());
            };
        } else
        {
            // Or else, the player has defeated all enemies in this level
            _healthHandler.OnDeath += () =>
            {
                StartCoroutine(FadeAwayCoroutine());
                LevelManager.Instance.SetState(new WinState());
            };
        }
    }

    protected override void RenderAttack()
    {
        if (_healthHandler.IsDead()) { return; }
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
        _nextEnemyObject.SetActive(true);
        yield return new WaitForSeconds(0.1f);
        float currTime = 0f;
        float timeToWait = TIME_TO_ANIMATE_NEXT_ENEMY_IN;
        Vector3 startPos = _nextEnemyObject.transform.position;
        while (currTime < timeToWait)
        {
            _nextEnemyObject.transform.position = Vector3.Lerp(startPos, transform.position, currTime / timeToWait);
            Debug.Log(currTime / timeToWait);
            currTime += Time.deltaTime; 
            yield return null;
        }
        yield return new WaitForSeconds(0.1f);
        LevelManager.Instance.SetNewEnemy(_nextEnemyObject.GetComponent<HealthHandler>());
        LevelManager.Instance.SetState(new PlayerTurnState());
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
            _healthHandler.TextToUpdate.color -= new Color(0, 0, 0, 0.04f);
            yield return new WaitForSeconds(0.02f);
        }
    }

}
