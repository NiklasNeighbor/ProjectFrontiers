using UnityEngine;
using System.Collections.Generic;
using TMPro;
using Unity.VisualScripting;

public class StashHandler : MonoBehaviour
{
    public InteractionManager interactionManager;
    public GameObject spawnableObject;
    public BoxCollider trigger; // The crate's trigger collider
    public TextMeshPro countertext;
    public int stashValue = 0;

    private List<GameObject> spawnedObjects = new List<GameObject>(); // Track all spawned objects

    public void AddValue(int value)
    {
        stashValue += value;
        Debug.Log($"Stash value: {stashValue}");
        if (countertext != null)
        {
            countertext.SetText(stashValue.ToString());
        }
        else
        {
            Debug.LogWarning("Countertext is not assigned!");
        }
    }
    private void Update()
    {
        countertext.SetText(stashValue.ToString());
    }

    public void SpawnObject()
    {
        if (stashValue > 0)
        {
            // Spawn the object at the specified location
            Vector3 spawnPosition = interactionManager.hit.point - interactionManager.playerCamera.transform.forward;
            GameObject newObject = Instantiate(spawnableObject, spawnPosition, Quaternion.identity);
            
            // Add to the list of spawned objects
            spawnedObjects.Add(newObject);

            // Configure interaction manager
            interactionManager.pickedObject = newObject;
            interactionManager.pickedRigidbody = newObject.GetComponent<Rigidbody>();
            interactionManager.Holding();

            // Deduct from stash value
            AddValue(-1);
        }
    }

    public void DeleteObject(GameObject objectToDelete)
    {
        if (objectToDelete != null)
        {
            // Remove from the list of spawned objects
            if (spawnedObjects.Contains(objectToDelete))
            {
                spawnedObjects.Remove(objectToDelete);
            }

            // Destroy the object and increment stash value
            Destroy(objectToDelete);
            AddValue(1);
            Debug.Log("Object dropped into stash and deleted.");
        }
        else
        {
            Debug.LogWarning("No valid object to delete.");
        }
    }

    // Trigger detection for objects inside the stash
    void OnTriggerStay(Collider other)
    {
        // Check if the object is one of the spawned objects and is not being held
        if (spawnedObjects.Contains(other.gameObject) && !interactionManager.isHolding)
        {
            Debug.Log($"Deleting object inside stash: {other.gameObject.name}");
            DeleteObject(other.gameObject); // Delete the object
        }
    }
}

