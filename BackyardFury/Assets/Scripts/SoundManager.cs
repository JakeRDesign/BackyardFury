using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Audio;

public class SoundManager : MonoBehaviour
{

    public AudioMixerGroup mixerGroup;
    public List<Sound> sounds;

    public static SoundManager instance;

    private void Awake()
    {
        // make sure only one instance of the soundmanager exists
        if (instance == null)
        {
            instance = this;
        }
        else
        {
            Destroy(this.gameObject);
            return;
        }

        DontDestroyOnLoad(this.gameObject);

        // create audio sources
        foreach(Sound s in sounds)
        {
            s.source = gameObject.AddComponent<AudioSource>();

            s.source.clip = s.clip;
            s.source.loop = s.loop;
            s.source.pitch = s.pitch;
            s.source.volume = s.volume;
            s.source.outputAudioMixerGroup = mixerGroup;
        }
    }

    public void Play(string name, float vol = 1.0f)
    {
        Sound s = GetSound(name);
        if (s != null)
        {
            if(s.randomPitch)
                s.source.pitch = Random.Range(s.minPitch, s.maxPitch);

            s.source.volume = s.volume * vol;

            s.source.Play();
        }
    }

    public void Pause(string name)
    {
        Sound s = GetSound(name);
        if (s != null)
            s.source.Pause();
    }

    public void Stop(string name)
    {
        Sound s = GetSound(name);
        if (s != null)
            s.source.Stop();
    }

    Sound GetSound(string name)
    {
        foreach (Sound s in sounds)
            if (s.name == name)
                return s;

        Debug.LogError("Tried to get sound " + name + " but it doesn't exist");
        return null;
    }


}

[System.Serializable]
public class Sound
{
    public AudioClip clip;
    [HideInInspector]
    public AudioSource source;

    public string name;

    [Range(0.0f, 1.0f)]
    public float volume = 1.0f;
    [Range(0.3f, 3.0f)]
    public float pitch = 1.0f;

    public bool loop = false;

    [Header("Randomise Pitch")]
    public bool randomPitch = false;
    [Range(0.3f, 3.0f)]
    public float minPitch = 0.3f;
    [Range(0.3f, 3.0f)]
    public float maxPitch = 3.0f;
}