using UnityEngine;

#if UNITY_EDITOR
public class DevToolsManager : Singleton<DevToolsManager>
{
    [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.AfterSceneLoad)]
    private static void EnsureInstanceExists()
    {
        if (Instance != null) { return; }
        GameObject go = new GameObject(nameof(DevToolsManager));
        go.AddComponent<DevToolsManager>();
        DontDestroyOnLoad(go);
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.F9))
        {
            KillCurrentEnemy();
        }
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
}
#endif
