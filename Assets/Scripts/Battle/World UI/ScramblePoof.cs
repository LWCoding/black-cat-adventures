using UnityEngine;

/// <summary>
/// A one-shot "poof" of cloud-like particles, spawned over a tile when it is
/// Scrambled. The whole effect is built in code so it needs no prefab and no
/// imported art to work: call <see cref="PlayAt"/> and it spawns, plays once,
/// and destroys itself.
///
/// An optional <paramref name="cloudSprite"/> can be passed to make the puff
/// read as a soft cloud; if none is provided it falls back to plain particles
/// using the always-available "Sprites/Default" shader.
/// </summary>
public static class ScramblePoof
{

    /// <summary>
    /// Spawns a short cloud-puff burst at <paramref name="worldPosition"/>.
    /// </summary>
    /// <param name="worldPosition">Where to play the effect (a tile's position).</param>
    /// <param name="cloudSprite">Optional sprite to texture the particles with.</param>
    /// <param name="sortingOrder">Render order so the puff draws above the tiles.</param>
    public static void PlayAt(Vector3 worldPosition, Sprite cloudSprite = null, int sortingOrder = 100)
    {
        GameObject go = new GameObject("ScramblePoof");
        go.transform.position = worldPosition;

        ParticleSystem ps = go.AddComponent<ParticleSystem>();
        // AddComponent auto-plays with default settings, so reset before we configure.
        ps.Stop(true, ParticleSystemStopBehavior.StopEmittingAndClear);

        ParticleSystem.MainModule main = ps.main;
        main.duration = 0.5f;
        main.loop = false;
        main.playOnAwake = false;
        main.startLifetime = new ParticleSystem.MinMaxCurve(0.35f, 0.55f);
        main.startSpeed = new ParticleSystem.MinMaxCurve(0.8f, 1.8f);
        main.startSize = new ParticleSystem.MinMaxCurve(0.35f, 0.7f);
        main.startColor = new Color(0.95f, 0.95f, 1f, 0.9f);
        main.gravityModifier = -0.15f;  // drift gently upward like a puff of smoke
        main.simulationSpace = ParticleSystemSimulationSpace.World;
        main.stopAction = ParticleSystemStopAction.Destroy;  // clean itself up when finished

        // No continuous emission: just a single burst of particles.
        ParticleSystem.EmissionModule emission = ps.emission;
        emission.rateOverTime = 0f;
        emission.SetBursts(new[] { new ParticleSystem.Burst(0f, (short)12) });

        // Emit from a small circle so the cloud has a bit of spread.
        ParticleSystem.ShapeModule shape = ps.shape;
        shape.enabled = true;
        shape.shapeType = ParticleSystemShapeType.Circle;
        shape.radius = 0.25f;

        // Fade the particles out over their lifetime.
        ParticleSystem.ColorOverLifetimeModule colorOverLifetime = ps.colorOverLifetime;
        colorOverLifetime.enabled = true;
        Gradient gradient = new Gradient();
        gradient.SetKeys(
            new[] { new GradientColorKey(Color.white, 0f), new GradientColorKey(Color.white, 1f) },
            new[] { new GradientAlphaKey(0.9f, 0f), new GradientAlphaKey(0f, 1f) });
        colorOverLifetime.color = new ParticleSystem.MinMaxGradient(gradient);

        // Grow slightly as they dissipate.
        ParticleSystem.SizeOverLifetimeModule sizeOverLifetime = ps.sizeOverLifetime;
        sizeOverLifetime.enabled = true;
        sizeOverLifetime.size = new ParticleSystem.MinMaxCurve(1f, AnimationCurve.EaseInOut(0f, 0.6f, 1f, 1.3f));

        // "Sprites/Default" is always available in this 2D project, so the
        // particles render correctly even when no cloud sprite is supplied.
        ParticleSystemRenderer renderer = go.GetComponent<ParticleSystemRenderer>();
        Material material = new Material(Shader.Find("Sprites/Default"));
        if (cloudSprite != null)
        {
            material.mainTexture = cloudSprite.texture;
        }
        renderer.material = material;
        renderer.sortingOrder = sortingOrder;

        ps.Play();
        // Safety net in case stopAction doesn't fire for any reason.
        Object.Destroy(go, 2f);
    }

}
