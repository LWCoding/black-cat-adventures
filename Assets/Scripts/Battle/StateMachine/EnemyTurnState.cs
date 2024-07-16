using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyTurnState : State
{

    public override void OnEnterState()
    {
        LevelManager.Instance.RenderEnemyDamage();
    }

    public override void OnExitState()
    {

    }

}
