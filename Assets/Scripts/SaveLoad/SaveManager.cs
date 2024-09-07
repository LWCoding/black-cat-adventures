using System.IO;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SaveManager
{

    public static void SaveGame(GameData gameData)
    {
        string json = JsonUtility.ToJson(gameData);
        File.WriteAllText(Application.persistentDataPath + "/savefile.json", json);
        Debug.Log("Saved game!");
    }

    public static GameData LoadGame()
    {
        if (File.Exists(Application.persistentDataPath + "/savefile.json"))
        {
            string json = File.ReadAllText(Application.persistentDataPath + "/savefile.json");
            return JsonUtility.FromJson<GameData>(json);
        }
        else
        {
            return null;
        }
    }

    public static void EraseSave()
    {
        File.Delete(Application.persistentDataPath + "/savefile.json");
        Debug.Log("Erased save!");
    }

}
