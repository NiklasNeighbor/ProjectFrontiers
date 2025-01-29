using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Rigidbody))]
[RequireComponent(typeof(Collider))]
public class WildResource : MonoBehaviour
{
    public StashHandler Destination;
    public InteractionManager InteractionManager;
    public GameObject vfxPrefab;
    public AudioClip pickUpSound;
    public float sfxVolume = 1f;

    private static AudioSource globalAudioSource; // Static reference to the global AudioSource

    void Start()
    {
        gameObject.tag = "Pickup";

        // Find or create the global AudioSource
        if (globalAudioSource == null)
        {
            GameObject audioSourceObject = new GameObject("GlobalAudioSource");
            globalAudioSource = audioSourceObject.AddComponent<AudioSource>();
            globalAudioSource.spatialBlend = 0; // Set to 2D audio
            DontDestroyOnLoad(audioSourceObject); // Make it persistent
        }
    }

    void Update()
    {
        checkForPickup();
    }

    void checkForPickup()
    {
        if (InteractionManager.isHolding)
        {
            if (InteractionManager.pickedObject == gameObject)
            {
                Debug.Log(InteractionManager.pickedObject + ", " + gameObject);

                // Instantiate the VFX at the position of the current object
                if (vfxPrefab != null)
                {
                    Instantiate(vfxPrefab, transform.position, transform.rotation);
                }

                // Play the pickup sound using the global AudioSource
                if (pickUpSound != null && globalAudioSource != null)
                {
                    globalAudioSource.PlayOneShot(pickUpSound, sfxVolume);
                }

                // Update the stash value
                Destination.stashValue = Destination.stashValue + 1;

                // Destroy the current object
                Destroy(gameObject);
            }
        }
    }
}
