using UnityEngine;
using UnityEngine.SceneManagement;

/// <summary>
/// Global audio service that survives scene loads. Owns two channels: a
/// one-shot SFX channel and a dedicated looping music channel. A single
/// instance is created automatically before the first scene loads, so it is
/// available everywhere without needing to be placed in any scene.
/// </summary>
[RequireComponent(typeof(AudioSource))]
public class AudioManager : PersistentSingleton<AudioManager>
{

    private AudioSource _sfxSource;
    private AudioSource _musicSource;

    // Name of the scene that was active when the current music track started.
    // The track keeps playing across reloads of that same scene, and is
    // stopped automatically once a different scene loads.
    private string _musicScene;

    [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.BeforeSceneLoad)]
    private static void EnsureInstanceExists()
    {
        if (Instance != null) { return; }
        GameObject go = new GameObject(nameof(AudioManager));
        go.AddComponent<AudioManager>();
    }

    protected override void OnPersistentAwake()
    {
        SetUpAudioSources();
        SceneManager.sceneLoaded += OnSceneLoaded;
    }

    private void OnDestroy()
    {
        if (Instance == this)
        {
            SceneManager.sceneLoaded -= OnSceneLoaded;
        }
    }

    private void SetUpAudioSources()
    {
        // SFX channel (the AudioSource added by [RequireComponent]).
        _sfxSource = GetComponent<AudioSource>();
        _sfxSource.playOnAwake = false;
        _sfxSource.spatialBlend = 0f;

        // Dedicated looping channel for background music.
        _musicSource = gameObject.AddComponent<AudioSource>();
        _musicSource.playOnAwake = false;
        _musicSource.loop = true;
        _musicSource.spatialBlend = 0f;
    }

    private void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        // Once we move to a scene different from the one that started the
        // current track, that music is no longer relevant, so stop it. Reloads
        // of the same scene keep the track playing seamlessly.
        if (_musicSource.isPlaying && scene.name != _musicScene)
        {
            StopMusic();
        }
    }

    /// <summary>
    /// Plays a one-shot sound effect on the SFX channel.
    /// </summary>
    public void PlayOneShot(AudioClip clip, float volume = 1f)
    {
        if (clip == null) { return; }
        _sfxSource.PlayOneShot(clip, volume);
    }

    /// <summary>
    /// Plays looping background music. If the requested clip is already
    /// playing this is a no-op, so the track continues seamlessly across scene
    /// reloads (e.g. advancing between enemies in the same battle scene).
    /// </summary>
    public void PlayMusic(AudioClip clip, float volume = 1f, bool loop = true)
    {
        if (clip == null) { return; }
        // Already playing this exact track: just refresh its home scene so it
        // survives reloads triggered from the (possibly new) active scene.
        if (_musicSource.isPlaying && _musicSource.clip == clip)
        {
            _musicScene = SceneManager.GetActiveScene().name;
            return;
        }
        _musicSource.clip = clip;
        _musicSource.loop = loop;
        _musicSource.volume = volume;
        _musicScene = SceneManager.GetActiveScene().name;
        _musicSource.Play();
    }

    /// <summary>
    /// Stops the currently playing background music, if any.
    /// </summary>
    public void StopMusic()
    {
        _musicSource.Stop();
        _musicSource.clip = null;
        _musicScene = null;
    }

}
