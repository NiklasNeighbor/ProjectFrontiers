using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AudioManager : MonoBehaviour
{
    public static AudioManager Instance { get; private set; }

    public MusicEntry[] MusicList;
    private Dictionary<string, AudioClip> musicDictionary;
    public AudioSource TargetAudioSource;

    private void Awake()
    {
        
        if (Instance == null)
        {
            Instance = this;
        }
        else
        {
            Destroy(gameObject); 
        }
    }



    // Start is called before the first frame update
    void Start()
    {
        musicDictionary = new Dictionary<string, AudioClip>();
        foreach (MusicEntry entry in MusicList)
        {
            if (!musicDictionary.ContainsKey(entry.SongKey))
            {
                musicDictionary.Add(entry.SongKey, entry.Music);
            } else
            {
                Debug.LogWarning("Duplicate Key for the Music found in AudioManager! Ensure each Key is unique!");
            }
        }
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void PlaySong(string Key)
    {
        TargetAudioSource.Stop();
        TargetAudioSource.clip = musicDictionary[Key];
        TargetAudioSource.Play();
    }
    public void StopSong()
    {
        TargetAudioSource.Stop();
    }




    //MOSTLY AI GENERATED CODE BELOW
    //I CAN NOT BE BOTHERED TO LEARN COROUTINES RIGHT NOW
    //WILL POSSIBLY BE REVISED LATER


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
