using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LoseState : State
{
    public override void OnEnterState()
    {
        LevelManager.Instance.ReloadCurrentLevel(1);
    }

    public override void OnExitState()
    {
        throw new System.NotImplementedException();
    }
}
