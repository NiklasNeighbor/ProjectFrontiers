using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AudioTester : MonoBehaviour
{
    public void PlaySong(string SongKey)
    {
        AudioManager.Instance.PlaySong(SongKey);
    }

    public void TransitionToSong(string SongKey)
    {
        AudioManager.Instance.Crossfade(SongKey, 2);
    }

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Alpha1))
        {
            PlaySong("TestSongA");
            Debug.Log("TestSongA now playing");
        }
        if (Input.GetKeyDown(KeyCode.Alpha2))
        {
            PlaySong("TestSongB");
            Debug.Log("TestSongB now Playing");
        }
        if (Input.GetKeyDown(KeyCode.Alpha3))
        {
            TransitionToSong("TestSongB");
            Debug.Log("Now transitioning to TestSongB");
        }
    }
}
