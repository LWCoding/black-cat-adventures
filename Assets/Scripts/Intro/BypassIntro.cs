using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BypassIntro : MonoBehaviour
{

    public bool IsActive = false;

    private void Update()
    {
        if (!IsActive) { return; }
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            // Skipping the intro on a new run drops straight into the tutorial battle.
            GameManager.EnterFirstTutorialBattle();
        }
    }

}
