using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerTurnState : State
{

    public override void OnEnterState()
    {
        BattleManager.Instance.PlayerHandler.StatusHandler.RenderStatusEffectEffects();
    }

    public override void OnExitState()
    {

    }

}
