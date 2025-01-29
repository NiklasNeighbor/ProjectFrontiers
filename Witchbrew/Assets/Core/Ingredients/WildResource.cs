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

    void Start()
    {
        gameObject.tag = "Pickup";
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

                if (pickUpSound != null)
                {
                    AudioSource.PlayClipAtPoint(pickUpSound, transform.position, sfxVolume);
                }

                // Update the stash value
                Destination.stashValue = Destination.stashValue + 1;

                // Destroy the current object
                Destroy(gameObject);
            }
        }
    }
}
