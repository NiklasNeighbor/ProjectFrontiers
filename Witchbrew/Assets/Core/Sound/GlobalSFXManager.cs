using UnityEngine;

public class GlobalSFXManager : MonoBehaviour
{
    public static GlobalSFXManager Instance { get; private set; }

    public AudioSource audioSource;

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
            audioSource = gameObject.AddComponent<AudioSource>();
            audioSource.spatialBlend = 0; // Set to 2D audio
        }
        else
        {
            Destroy(gameObject);
        }
    }
}