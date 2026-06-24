using UnityEngine;

/// <summary>
/// Sits on each pooled particle GameObject alongside its <see cref="ParticleSystem"/>.
/// Caches the <see cref="ParticleSystem"/> and its renderer material so
/// <see cref="ParticleManager"/> can reconfigure and replay the object without
/// re-adding components. Returns itself to the pool automatically when the
/// particle system finishes playing.
/// </summary>
[RequireComponent(typeof(ParticleSystem))]
public class PooledParticle : MonoBehaviour
{

    public ParticleSystem PS { get; private set; }
    public ParticleSystemRenderer PSRenderer { get; private set; }
    public Material PSMaterial { get; private set; }

    private void Awake()
    {
        PS = GetComponent<ParticleSystem>();
        PSRenderer = GetComponent<ParticleSystemRenderer>();
        PSMaterial = new Material(Shader.Find("Sprites/Default"));
        PSRenderer.material = PSMaterial;
    }

    /// <summary>
    /// Called by Unity when the particle system finishes (requires
    /// <c>main.stopAction = ParticleSystemStopAction.Callback</c>).
    /// Returns this object to the pool.
    /// </summary>
    private void OnParticleSystemStopped()
    {
        if (ParticleManager.Instance != null)
        {
            ParticleManager.Instance.Return(this);
        }
        else
        {
            gameObject.SetActive(false);
        }
    }

}
