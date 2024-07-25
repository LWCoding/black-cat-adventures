using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyTurnState : State
{

    public override void OnEnterState()
    {
        if (LevelManager.Instance.CurrEnemyHandler.HealthHandler.IsDead()) { return; }  // If dead, don't render!
        LevelManager.Instance.OnEnemyAttack?.Invoke();
    }

    public override void OnExitState()
    {

    }

}
