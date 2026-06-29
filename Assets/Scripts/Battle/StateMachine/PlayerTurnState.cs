using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerTurnState : State
{

    private readonly bool _tickStatuses;

    public PlayerTurnState(bool tickStatuses = true)
    {
        _tickStatuses = tickStatuses;
    }

    public override void OnEnterState()
    {
        if (!_tickStatuses) { return; }
        var sh = BattleManager.Instance.PlayerHandler.StatusHandler;
        bool stunned = sh.HasStatus(StatusEffectType.Stunned);
        sh.RenderStatusEffectEffects();
        if (stunned)
        {
            BattleManager.Instance.SetState(new EnemyTurnState());
        }
    }

    public override void OnExitState()
    {
        WordGrid.Instance.TickLockedTiles();
    }

}
