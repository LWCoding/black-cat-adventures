using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class BypassIntro : MonoBehaviour
{

    public bool IsActive = false;

    private void Update()
    {
        if (!IsActive) { return; }
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            SceneManager.LoadScene("Map");
        }
    }

}
