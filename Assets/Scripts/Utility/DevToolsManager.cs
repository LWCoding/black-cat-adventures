using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.SceneManagement;

#if UNITY_EDITOR
[SelfSpawning(RuntimeInitializeLoadType.AfterSceneLoad, KeepAcrossScenes = true)]
public class DevToolsManager : Singleton<DevToolsManager>
{
    private EnemyData[] _enemyLibrary;
    private int _libraryIndex;

    private EnemyData[] EnemyLibrary
    {
        get
        {
            if (_enemyLibrary == null)
            {
                _enemyLibrary = Resources.LoadAll<EnemyData>("ScriptableObjects/Characters")
                    .OrderBy(e => e.name)
                    .ToArray();
            }
            return _enemyLibrary;
        }
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.F9))
        {
            KillCurrentEnemy();
        }
        if (Input.GetKeyDown(KeyCode.F7))
        {
            SkipCurrentLevel();
        }
        if (Input.GetKeyDown(KeyCode.F5))
        {
            CycleEnemy(+1);
        }
        if (Input.GetKeyDown(KeyCode.F4))
        {
            CycleEnemy(-1);
        }
        if (Input.GetKeyDown(KeyCode.F2))
        {
            GiveAllTreasures();
        }
    }

    // Cheat: marks the level/node complete via the normal WinState flow, then
    // jumps straight back to the map (skipping the win screen delay).
    private void SkipCurrentLevel()
    {
        if (BattleManager.Instance != null)
        {
            BattleManager.Instance.SetState(new WinState());
        }
        SaveManager.SaveGame(GameManager.GameData);
        SceneManager.LoadScene("Map");
    }

    // Cheat: routes full damage through the normal TakeDamage flow so
    // OnDeath / state transitions fire exactly as they would on a real kill.
    private void KillCurrentEnemy()
    {
        EnemyHandler enemy = BattleManager.Instance?.CurrEnemyHandler;
        if (enemy == null) { return; }
        HealthHandler health = enemy.HealthHandler;
        if (health == null || health.IsDead()) { return; }
        health.TakeDamage(health.CurrentHealth);
    }

    // Cheat: gives the player every treasure (minus the None placeholder), saves, and
    // refreshes the inventory if the Map scene is currently active.
    private void GiveAllTreasures()
    {
        List<Treasure> all = Resources.LoadAll<Treasure>("ScriptableObjects/Treasure")
            .Where(t => t is not None)
            .ToList();
        GameManager.GameData.UnlockedTreasures = all;
        SaveManager.SaveGame(GameManager.GameData);
        InventoryManager.Instance?.RefreshFromGameData();
    }

    // Cheat: replaces the current enemy's data with the next/previous entry in the
    // enemy library (alphabetical order), reloading sprite, attacks, and health.
    private void CycleEnemy(int direction)
    {
        EnemyHandler enemy = BattleManager.Instance?.CurrEnemyHandler;
        if (enemy == null) { return; }

        EnemyData[] lib = EnemyLibrary;
        if (lib.Length == 0) { return; }

        int found = System.Array.IndexOf(lib, enemy.CharData as EnemyData);
        if (found >= 0) { _libraryIndex = found; }

        int len = lib.Length;
        _libraryIndex = ((_libraryIndex + direction) % len + len) % len;

        EnemyData next = lib[_libraryIndex];
        enemy.SetCharacterData(next);
        EnemyInfoBox.Instance?.SetInfo(next);
    }
}
#endif
