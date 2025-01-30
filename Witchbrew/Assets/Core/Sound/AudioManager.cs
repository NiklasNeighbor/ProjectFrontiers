using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class AudioManager : MonoBehaviour
{
    public static AudioManager Instance { get; private set; }

    public MusicEntry[] MusicList;
    private Dictionary<string, AudioClip> musicDictionary;
    public AudioSource TargetAudioSource;

    public SceneMusicConfig[] sceneMusicConfigs; // Array of scene-specific music configurations

    private void Awake()
    {
        // If no instance exists, set this object as the instance and don't destroy on load
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject); // Prevents the object from being destroyed when loading a new scene
        }
        else
        {
            // If an instance already exists, destroy the new duplicate
            Destroy(gameObject);
            return;
        }

        InitializeMusicDictionary(); // Initialize the dictionary in Awake
        SceneManager.sceneLoaded += OnSceneLoaded; // Subscribe to the sceneLoaded event
    }

    private void InitializeMusicDictionary()
    {
        if (MusicList == null || MusicList.Length == 0)
        {
            Debug.LogError("MusicList is null or empty!");
            return;
        }

        Debug.Log($"MusicList has {MusicList.Length} entries.");

        musicDictionary = new Dictionary<string, AudioClip>();
        foreach (MusicEntry entry in MusicList)
        {
            if (entry == null)
            {
                Debug.LogWarning("Found a null entry in MusicList!");
                continue;
            }

            if (string.IsNullOrEmpty(entry.SongKey))
            {
                Debug.LogWarning("Found an entry with an empty or null SongKey!");
                continue;
            }

            if (entry.Music == null)
            {
                Debug.LogWarning($"Music is null for SongKey: {entry.SongKey}");
                continue;
            }

            if (!musicDictionary.ContainsKey(entry.SongKey))
            {
                musicDictionary.Add(entry.SongKey, entry.Music);
            }
            else
            {
                Debug.LogWarning($"Duplicate Key for the Music found in AudioManager: {entry.SongKey}");
            }
        }

        Debug.Log($"musicDictionary has {musicDictionary.Count} entries.");
    }

    private void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        PlaySceneSong(); // Play the appropriate song for the new scene
    }

    private void OnDestroy()
    {
        // Unsubscribe from the event to avoid memory leaks
        SceneManager.sceneLoaded -= OnSceneLoaded;
    }

    void Start()
    {
        if (MusicList == null || MusicList.Length == 0)
        {
            Debug.LogError("MusicList is null or empty!");
            return;
        }

        Debug.Log($"MusicList has {MusicList.Length} entries.");

        musicDictionary = new Dictionary<string, AudioClip>();
        foreach (MusicEntry entry in MusicList)
        {
            if (entry == null)
            {
                Debug.LogWarning("Found a null entry in MusicList!");
                continue;
            }

            if (string.IsNullOrEmpty(entry.SongKey))
            {
                Debug.LogWarning("Found an entry with an empty or null SongKey!");
                continue;
            }

            if (entry.Music == null)
            {
                Debug.LogWarning($"Music is null for SongKey: {entry.SongKey}");
                continue;
            }

            if (!musicDictionary.ContainsKey(entry.SongKey))
            {
                musicDictionary.Add(entry.SongKey, entry.Music);
            }
            else
            {
                Debug.LogWarning($"Duplicate Key for the Music found in AudioManager: {entry.SongKey}");
            }
        }

        Debug.Log($"musicDictionary has {musicDictionary.Count} entries.");

        // Play the appropriate song for the current scene
        PlaySceneSong();
    }

    private void PlaySceneSong()
    {
        if (sceneMusicConfigs == null || sceneMusicConfigs.Length == 0)
        {
            Debug.LogError("sceneMusicConfigs is null or empty!");
            return;
        }

        string currentSceneName = SceneManager.GetActiveScene().name;
        Debug.Log($"Current scene: {currentSceneName}");

        // Find the song key for the current scene
        foreach (var config in sceneMusicConfigs)
        {
            if (config == null)
            {
                Debug.LogWarning("Found a null entry in sceneMusicConfigs!");
                continue;
            }

            if (config.sceneName == currentSceneName)
            {
                if (string.IsNullOrEmpty(config.songKey))
                {
                    Debug.LogWarning($"songKey is null or empty for scene: {currentSceneName}");
                    return;
                }

                Debug.Log($"Playing song for scene: {currentSceneName}, SongKey: {config.songKey}");
                PlaySong(config.songKey);
                return;
            }
        }

        Debug.LogWarning($"No song configured for scene: {currentSceneName}");
    }

    public void PlaySong(string Key)
    {
        if (TargetAudioSource == null)
        {
            Debug.LogError("TargetAudioSource is null!");
            return;
        }

        if (musicDictionary == null)
        {
            Debug.LogError("musicDictionary is null!");
            return;
        }

        if (!musicDictionary.ContainsKey(Key))
        {
            Debug.LogWarning("Song key not found: " + Key);
            return;
        }

        if (musicDictionary[Key] == null)
        {
            Debug.LogError($"AudioClip for key '{Key}' is null!");
            return;
        }

        TargetAudioSource.Stop();
        TargetAudioSource.clip = musicDictionary[Key];
        TargetAudioSource.Play();
    }

    public bool IsPlaying(string songKey)
    {
        if (musicDictionary.ContainsKey(songKey))
        {
            return TargetAudioSource.clip == musicDictionary[songKey];
        }
        Debug.LogWarning($"Song key not found: {songKey}");
        return false;
    }

    public void StopSong()
    {
        TargetAudioSource.Stop();
    }

    public void Crossfade(string Key, float duration = 1.5f)
    {
        AudioClip newClip = musicDictionary[Key] as AudioClip;
        StartCoroutine(FadeTransition(newClip, duration));
    }

    private IEnumerator FadeTransition(AudioClip newClip, float duration)
    {
        if (TargetAudioSource.clip == newClip) yield break; // Avoid unnecessary transitions

        float startVolume = TargetAudioSource.volume;

        // Fade out
        for (float t = 0; t < duration / 2; t += Time.deltaTime)
        {
            TargetAudioSource.volume = Mathf.Lerp(startVolume, 0, t / (duration / 2));
            yield return null;
        }
        TargetAudioSource.volume = 0;
        TargetAudioSource.Stop();

        // Swap clips and play
        TargetAudioSource.clip = newClip;
        TargetAudioSource.Play();

        // Fade in
        for (float t = 0; t < duration / 2; t += Time.deltaTime)
        {
            TargetAudioSource.volume = Mathf.Lerp(0, startVolume, t / (duration / 2));
            yield return null;
        }
        TargetAudioSource.volume = startVolume;
    }
}
