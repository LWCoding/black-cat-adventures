using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerTurnState : State
{

    public override void OnEnterState()
    {

    }

    public override void OnExitState()
    {
        // Make sure all tiles in the preview are erased
        WordPreview.Instance.EraseTiles();
    }

}
