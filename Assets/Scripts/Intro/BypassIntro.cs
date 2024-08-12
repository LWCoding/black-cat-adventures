using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class BypassIntro : MonoBehaviour
{

    [Header("Properties")]
    [SerializeField] private string _sceneNameAfterCutscene;

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            SceneManager.LoadScene(_sceneNameAfterCutscene);
        }
    }

}
