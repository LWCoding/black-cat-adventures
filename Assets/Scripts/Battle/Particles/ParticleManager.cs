using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Singleton that owns a reusable pool of <see cref="PooledParticle"/> objects
/// and exposes named spawn functions for each visual effect. Each pooled object
/// is a child GameObject with a <see cref="ParticleSystem"/> and a
/// <see cref="PooledParticle"/> component; it is deactivated and re-enqueued
/// automatically when its particles finish playing.
///
/// To add a new effect, add a <c>public void SpawnX(Vector3 position, ...)</c>
/// method that calls <see cref="Get"/>, a private <c>ConfigureAsX</c> that sets
/// all particle properties, then activates and plays the object.
/// </summary>
public class ParticleManager : Singleton<ParticleManager>
{

    [Header("Pool Settings")]
    [SerializeField] private int _prewarmCount = 8;

    [Header("Poof Effect")]
    [Tooltip("Optional cloud sprite for the poof effect; leave empty to use plain white particles.")]
    [SerializeField] private Sprite _poofSprite;

    private readonly Queue<PooledParticle> _pool = new();

    protected override void Awake()
    {
        base.Awake();
        for (int i = 0; i < _prewarmCount; i++)
        {
            PooledParticle p = CreatePooledObject();
            p.gameObject.SetActive(false);
            _pool.Enqueue(p);
        }
    }

    // -------------------------------------------------------------------------
    // Pool internals
    // -------------------------------------------------------------------------

    private PooledParticle CreatePooledObject()
    {
        GameObject go = new GameObject("PooledParticle");
        go.transform.SetParent(transform, false);
        // RequireComponent on PooledParticle adds ParticleSystem automatically.
        PooledParticle p = go.AddComponent<PooledParticle>();
        return p;
    }

    private PooledParticle Get()
    {
        PooledParticle p = _pool.Count > 0 ? _pool.Dequeue() : CreatePooledObject();
        p.PS.Stop(true, ParticleSystemStopBehavior.StopEmittingAndClear);
        return p;
    }

    /// <summary>
    /// Returns a finished particle object to the pool. Called automatically by
    /// <see cref="PooledParticle.OnParticleSystemStopped"/>; do not call manually.
    /// </summary>
    public void Return(PooledParticle p)
    {
        p.gameObject.SetActive(false);
        _pool.Enqueue(p);
    }

    // -------------------------------------------------------------------------
    // Public spawn methods
    // -------------------------------------------------------------------------

    /// <summary>
    /// Spawns a cloud-poof burst at <paramref name="position"/>. Uses the
    /// optional <paramref name="sprite"/> override, or falls back to the
    /// manager's default <c>_poofSprite</c>, or plain white particles if
    /// neither is set.
    /// </summary>
    public void SpawnPoof(Vector3 position, Sprite sprite = null)
    {
        PooledParticle p = Get();
        ConfigureAsPoof(p, sprite ?? _poofSprite);
        p.transform.position = position;
        p.gameObject.SetActive(true);
        p.PS.Play();
    }

    // -------------------------------------------------------------------------
    // Effect configuration
    // -------------------------------------------------------------------------

    private void ConfigureAsPoof(PooledParticle p, Sprite sprite)
    {
        ParticleSystem.MainModule main = p.PS.main;
        main.duration = 1.0f;
        main.loop = false;
        main.playOnAwake = false;
        main.startLifetime = new ParticleSystem.MinMaxCurve(0.85f, 1.05f);
        main.startSpeed = new ParticleSystem.MinMaxCurve(0.8f, 1.8f);
        main.startSize = new ParticleSystem.MinMaxCurve(0.35f, 0.7f);
        main.startColor = new Color(0.95f, 0.95f, 1f, 0.9f);
        main.gravityModifier = -0.15f;
        main.simulationSpace = ParticleSystemSimulationSpace.World;
        main.stopAction = ParticleSystemStopAction.Callback;

        ParticleSystem.EmissionModule emission = p.PS.emission;
        emission.rateOverTime = 0f;
        emission.SetBursts(new[] { new ParticleSystem.Burst(0f, (short)12) });

        ParticleSystem.ShapeModule shape = p.PS.shape;
        shape.enabled = true;
        shape.shapeType = ParticleSystemShapeType.Circle;
        shape.radius = 0.25f;

        ParticleSystem.ColorOverLifetimeModule colorOverLifetime = p.PS.colorOverLifetime;
        colorOverLifetime.enabled = true;
        Gradient gradient = new Gradient();
        gradient.SetKeys(
            new[] { new GradientColorKey(Color.white, 0f), new GradientColorKey(Color.white, 1f) },
            new[] { new GradientAlphaKey(0.9f, 0f), new GradientAlphaKey(0f, 1f) });
        colorOverLifetime.color = new ParticleSystem.MinMaxGradient(gradient);

        ParticleSystem.SizeOverLifetimeModule sizeOverLifetime = p.PS.sizeOverLifetime;
        sizeOverLifetime.enabled = true;
        sizeOverLifetime.size = new ParticleSystem.MinMaxCurve(1f, AnimationCurve.EaseInOut(0f, 0.6f, 1f, 1.3f));

        p.PSMaterial.mainTexture = sprite != null ? sprite.texture : null;
        p.PSRenderer.material = p.PSMaterial;
        p.PSRenderer.sortingOrder = 100;
    }

}
