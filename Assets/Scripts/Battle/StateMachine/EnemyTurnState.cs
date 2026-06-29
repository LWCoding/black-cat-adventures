using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyTurnState : State
{

    public override void OnEnterState()
    {
        if (BattleManager.Instance.CurrEnemyHandler.HealthHandler.IsDead()) { return; }
        bool stunned = BattleManager.Instance.CurrEnemyHandler.StatusHandler.HasStatus(StatusEffectType.Stunned);
        BattleManager.Instance.CurrEnemyHandler.StatusHandler.RenderStatusEffectEffects();
        if (BattleManager.Instance.CurrEnemyHandler.HealthHandler.IsDead()) { return; }
        if (stunned)
        {
            BattleManager.Instance.SetState(new PlayerTurnState());
            return;
        }
        BattleManager.Instance.OnEnemyAttack?.Invoke();
    }

    public override void OnExitState()
    {

    }

}
