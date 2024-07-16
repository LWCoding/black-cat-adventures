using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerHandler : CharacterHandler
{ 
    private void Start()
    {
        LevelManager.Instance.OnPlayerAttack += RenderAttack;
        HealthHandler.OnDeath += () =>
        {
            LevelManager.Instance.SetState(new LoseState());
        };
    }

    protected override void RenderAttack()
    {
        if (HealthHandler.IsDead()) { return; }
        LevelManager.Instance.SetState(new WaitState());
        StartCoroutine(RenderAttackCoroutine(() =>
        {
            if (LevelManager.Instance.CurrentState is WaitState)
            {
                LevelManager.Instance.SetState(new EnemyTurnState());
            }
        }));
    }

    protected override IEnumerator RenderAttackCoroutine(Action codeToRunAfter)
    {
        Vector3 startingPos = transform.position;
        SetSprite(_charData.AttackSprite);
        for (int i = 0; i < 10; i++)
        {
            transform.position += new Vector3(0.15f, 0, 0);
            yield return new WaitForSeconds(0.01f);
        }
        yield return new WaitForSeconds(0.1f);
        for (int i = 0; i < 10; i++)
        {
            transform.position -= new Vector3(0.15f, 0, 0);
            yield return new WaitForSeconds(0.01f);
        }
        transform.position = startingPos;
        SetSprite(_charData.AliveSprite);
        yield return new WaitForSeconds(0.2f);

        codeToRunAfter.Invoke();
    }

}
