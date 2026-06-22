using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class BypassIntro : MonoBehaviour
{

    [Header("Properties")]
    [SerializeField] private string _levelIdAfterCutscene;

    public bool IsActive = false;

    private void Update()
    {
        if (!IsActive) { return; }
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            GameManager.GameData.RecentLevelCompleted = _levelIdAfterCutscene;
            SceneManager.LoadScene("Level");
        }
    }

}
