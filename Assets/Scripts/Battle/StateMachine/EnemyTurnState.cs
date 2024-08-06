using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyTurnState : State
{

    public override void OnEnterState()
    {
        if (BattleManager.Instance.CurrEnemyHandler.HealthHandler.IsDead()) { return; }  // If dead, don't render!
        BattleManager.Instance.CurrEnemyHandler.StatusHandler.RenderStatusEffectEffects();
        if (BattleManager.Instance.CurrEnemyHandler.HealthHandler.IsDead()) { return; }  // If dead, don't render!
        BattleManager.Instance.OnEnemyAttack?.Invoke();
    }

    public override void OnExitState()
    {

    }

}
