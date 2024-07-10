using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyTurnState : State
{

    public override void OnEnterState()
    {
        Debug.Log("Enemy turn!");
    }

    public override void OnExitState()
    {
        Debug.Log("No longer enemy.");
    }

}
