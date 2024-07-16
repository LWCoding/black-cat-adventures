using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// This state is purely a placeholder while other
/// things are being animated.
/// </summary>
public class WaitState : State
{

    public override void OnEnterState()
    {
        Debug.Log("Waiting state...");
    }

    public override void OnExitState()
    {
        Debug.Log("Exit waiting state!");
    }

}
