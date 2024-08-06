using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerHandler : CharacterHandler
{ 
    private void Start()
    {
        BattleManager.Instance.OnPlayerAttack += RenderAttack;
        HealthHandler.OnDeath += () =>
        {
            BattleManager.Instance.SetState(new LoseState());
        };
    }

    protected override void RenderAttack()
    {
        if (HealthHandler.IsDead()) { return; }
        BattleManager.Instance.SetState(new WaitState());
        StartCoroutine(RenderAttackCoroutine(() =>
        {
            if (BattleManager.Instance.CurrentState is WaitState)
            {
                BattleManager.Instance.SetState(new EnemyTurnState());
            }
        }));
    }

    protected override IEnumerator RenderAttackCoroutine(Action codeToRunAfter)
    {
        Vector3 startingPos = transform.position;
        SetSprite(CharData.AttackSprite);
        float timeToWait = 0.1f;
        Vector3 targetPos = startingPos + new Vector3(1.5f, 0);
        while (timeToWait > 0) 
        {
            timeToWait -= Time.deltaTime;
            transform.position = Vector3.Lerp(startingPos, targetPos, (0.1f - timeToWait) * 10);
            yield return null;
        }
        yield return new WaitForSeconds(0.1f);
        timeToWait = 0.1f;
        while (timeToWait > 0)
        {
            timeToWait -= Time.deltaTime;
            transform.position = Vector3.Lerp(targetPos, startingPos, (0.1f - timeToWait) * 10);
            yield return null;
        }
        SetSprite(CharData.AliveSprite);
        yield return new WaitForSeconds(0.2f);

        codeToRunAfter.Invoke();
    }

}
