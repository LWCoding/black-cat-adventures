using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "Level Registry", menuName = "Levels/Level Registry")]
public class LevelRegistry : ScriptableObject
{

    [System.Serializable]
    public class Entry
    {
        public string LevelId;
        public LevelData LevelData;
    }

    public List<Entry> Entries = new();

    public LevelData GetLevelData(string levelId) => Entries.Find(e => e.LevelId == levelId)?.LevelData;

}
