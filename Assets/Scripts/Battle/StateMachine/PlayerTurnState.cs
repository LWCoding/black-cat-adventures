using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerTurnState : State
{

    public override void OnEnterState()
    {
        LevelManager.Instance.PlayerHandler.RenderStatusEffectEffects();
    }

    public override void OnExitState()
    {

    }

}
