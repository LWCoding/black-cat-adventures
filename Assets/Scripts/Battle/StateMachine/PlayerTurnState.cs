using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerTurnState : State
{

    public override void OnEnterState()
    {
        Debug.Log("Player turn!");
    }

    public override void OnExitState()
    {
        Debug.Log("No longer player.");
    }

}
