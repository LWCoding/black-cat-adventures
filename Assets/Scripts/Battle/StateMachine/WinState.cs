using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class WinState : State
{

    public override void OnEnterState()
    {
        // Register that we won this level
        GameManager.LevelsCompleted.Add(SceneManager.GetActiveScene().name);
    }

    public override void OnExitState()
    {

    }

}
