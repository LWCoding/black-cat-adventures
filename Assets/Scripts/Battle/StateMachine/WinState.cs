using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class WinState : State
{

    public override void OnEnterState()
    {
        Debug.Log("You won!");
        SceneManager.LoadScene("Map");
    }

    public override void OnExitState()
    {
        throw new System.NotImplementedException();
    }

}
