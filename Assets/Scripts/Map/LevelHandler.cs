using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class LevelHandler : MonoBehaviour
{

    [Header("Level Properties")]
    public string SceneName;

    /// <summary>
    /// Loads this level's information and appropriate scene.
    /// </summary>
    public void LoadLevel()
    {
        SceneManager.LoadScene(SceneName);
    }

}
