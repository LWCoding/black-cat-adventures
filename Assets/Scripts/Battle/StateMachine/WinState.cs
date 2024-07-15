using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WinState : State
{

    public override void OnEnterState()
    {
        Debug.Log("You won!");
    }

    public override void OnExitState()
    {
        throw new System.NotImplementedException();
    }

}
