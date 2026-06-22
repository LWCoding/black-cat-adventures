using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WinState : State
{

    public override void OnEnterState()
    {
        // Register that we won this level
        GameManager.GameData.LevelsCompleted.Add(GameManager.GameData.RecentLevelCompleted);
    }

    public override void OnExitState()
    {

    }

}
