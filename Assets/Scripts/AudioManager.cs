using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(AudioSource))]
public class AudioManager : MonoBehaviour
{

    private static AudioManager _instance;
    public static AudioManager Instance
    {
        get
        {
            if (_instance == null)
            {
                _instance = FindAnyObjectByType<AudioManager>();
            }
            return _instance;
        }
    }

    private AudioSource _audioSource;

    private void Awake()
    {
        if (!ReferenceEquals(_instance, this))
        {
            if (_instance != null)
            {
                Destroy(this);
            }
            else
            {
                _instance = this;
            }
        }
        // Set the audio source.
        _audioSource = GetComponent<AudioSource>();
    }

    public void PlayOneShot(AudioClip clip, float volume = 1f)
    {
        _audioSource.PlayOneShot(clip, volume);
    }

}
