using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class EraseSave : MonoBehaviour
{
    
    /// <summary>
    /// Delete the save file AND reset the player's information.
    /// </summary>
    public void DeleteSaveFile()
    {
        SaveManager.EraseSave();
        GameManager.GameData = new();
        SceneManager.LoadScene("Intro");
    }

}
