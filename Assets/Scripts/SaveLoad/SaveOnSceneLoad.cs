using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SaveOnSceneLoad : MonoBehaviour
{

    private void Start()
    {
        SaveManager.SaveGame(GameManager.GameData);
    }

}
