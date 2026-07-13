using UnityEngine;
using System.Collections.Generic;

public class AudioManager : MonoBehaviour
{
    public static AudioManager Instance;

    [System.Serializable]
    public class Sound
    {
        public string name;
        public AudioClip clip;

        [Range(0f, 1f)]
        public float volume = 1f;

        public bool loop = false;
    }

    [Header("Sounds")]
    [SerializeField] private List<Sound> sounds = new List<Sound>();

    private readonly Dictionary<string, AudioSource> audioSources = new();

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
            return;
        }

        foreach (Sound sound in sounds)
        {
            AudioSource source = gameObject.AddComponent<AudioSource>();

            source.clip = sound.clip;
            source.volume = sound.volume;
            source.loop = sound.loop;

            audioSources.Add(sound.name, source);
        }

        Play("BackgroundMusic");
    }

    public void Play(string soundName)
    {
        if (!audioSources.TryGetValue(soundName, out AudioSource source))
        {
            Debug.LogWarning($"Sound not found: {soundName}");
            return;
        }

        if (source.loop)
        {
            if (!source.isPlaying)
            {
                source.Play();
            }
        }
        else
        {
            source.PlayOneShot(source.clip);
        }
    }

    public void Stop(string soundName)
    {
        if (audioSources.TryGetValue(soundName, out AudioSource source))
        {
            source.Stop();
        }
    }
}