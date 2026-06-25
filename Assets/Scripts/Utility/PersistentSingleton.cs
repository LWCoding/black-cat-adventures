using UnityEngine;

/// <summary>
/// A <see cref="Singleton{T}"/> whose instance survives scene loads. The first
/// instance to awake becomes the persistent one (kept alive via
/// <c>DontDestroyOnLoad</c>); any duplicate that awakes later (for example a
/// stray copy left in a scene) is silently ignored — the base <see cref="Singleton{T}"/>
/// already destroys the duplicate component, so no extra cleanup is needed here.
///
/// IMPORTANT: do NOT destroy the duplicate's whole GameObject. Scene-placed components
/// of this type may share a GameObject with unrelated components (e.g. AudioManager
/// and BattleManager on the same GameObject). Destroying the whole object would
/// silently remove those other components too.
///
/// Subclasses that want an instance to exist even when none is placed in a
/// scene should decorate the class with <see cref="SelfSpawningAttribute"/>, e.g.:
/// <code>
/// [SelfSpawning(RuntimeInitializeLoadType.BeforeSceneLoad)]
/// public class MyManager : PersistentSingleton&lt;MyManager&gt; { ... }
/// </code>
/// The <see cref="SelfSpawningBootstrap"/> scanner handles the actual
/// instantiation. (<c>RuntimeInitializeOnLoadMethod</c> is not invoked on
/// generic types, so it cannot live on this base class — that is why a
/// separate non-generic bootstrap is used.)
/// </summary>
public abstract class PersistentSingleton<T> : Singleton<T> where T : MonoBehaviour
{

    protected sealed override void Awake()
    {
        // base.Awake() registers the first instance and calls Destroy(this) on
        // any duplicate component, so there is nothing left to do for duplicates.
        base.Awake();
        if (!ReferenceEquals(Instance, this)) { return; }
        DontDestroyOnLoad(gameObject);
        OnPersistentAwake();
    }

    /// <summary>
    /// Called once, on the surviving instance, after it has been marked
    /// <c>DontDestroyOnLoad</c>. Override this instead of <c>Awake</c> for setup.
    /// </summary>
    protected virtual void OnPersistentAwake() { }

}
