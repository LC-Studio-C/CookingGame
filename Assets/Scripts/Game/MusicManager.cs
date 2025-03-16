using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MusicManager : MonoBehaviour
{
    private static MusicManager instance;

    private AudioSource audioSource;

    private float volume=.5f;

    private const string PRE_MUSIC_VOLUME = "MusicEffectVolume";

    private void Awake()
    {
        if (instance != null)
        {
            Destroy(instance);
        }
        else
        {
            instance = this;
        }
        volume = PlayerPrefs.GetFloat(PRE_MUSIC_VOLUME);
    }

    private void Start()
    {
        audioSource = GetComponent<AudioSource>();
    }

    private void Update()
    {
        audioSource.volume = this.volume;
    }

    public void MusicVolumeChange()
    {
        volume += .1f;
        if (volume > 1.1f)
        {
            volume = 0f;
        }
        PlayerPrefs.SetFloat(PRE_MUSIC_VOLUME,volume);
    }

    public float GetVolume()
    {
        return volume;
    }

    public static MusicManager GetInstance()
    {
        return instance;
    }
}
