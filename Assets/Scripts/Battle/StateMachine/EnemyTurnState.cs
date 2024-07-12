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
        LevelManager.Instance.MakePlayerTakeDamage(4);
        Debug.Log("Dealt damage. No longer enemy.");
    }

}
