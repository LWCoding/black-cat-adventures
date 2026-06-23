using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WinState : State
{

    public override void OnEnterState()
    {
        GameManager.GameData.LevelsCompleted.Add(GameManager.GameData.RecentLevelCompleted);
        if (!string.IsNullOrEmpty(GameManager.GameData.RecentNodeEntered))
        {
            GameManager.GameData.CompletedNodeIds.Add(GameManager.GameData.RecentNodeEntered);
        }
    }

    public override void OnExitState()
    {

    }

}
