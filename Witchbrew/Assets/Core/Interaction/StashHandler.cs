using UnityEngine;

public class StashHandler : MonoBehaviour
{
public InteractionManager interactionManager;
    public GameObject spawnableObject;
    public int stashValue = 0;
    public float requiredTime = 1f;

    private float collisionTimer = 0f; // Tracks time spent colliding
    private GameObject currentIngredient = null; // Current object being held
    private GameObject spawnedObject = null; // The spawned object reference

    void Update()
    {
        // Check if we're holding an object and it's colliding with the stash
        if (currentIngredient != null && interactionManager.isHolding)
        {
            collisionTimer += Time.deltaTime;

            // Delete the object if the required time has passed
            if (collisionTimer >= requiredTime && currentIngredient == spawnedObject)
            {
                DeleteObject(); // Delete the held object after the required time
            }
        }
    }

    public void AddValue(int value)
    {
        stashValue += value;
        Debug.Log($"Stash value: {stashValue}");
    }

    public void SpawnObject()
    {
        if (stashValue > 0)
        {
            // Spawn the object at the specified location
            spawnedObject = Instantiate(spawnableObject, interactionManager.hit.point, Quaternion.identity);
            interactionManager.pickedObject = spawnedObject;
            interactionManager.pickedRigidbody = spawnedObject.GetComponent<Rigidbody>();
            interactionManager.Holding();
            AddValue(-1);
        }
    }

    public void DeleteObject()
    {
        // Only delete if the current held object is the same as the spawned one
        if (currentIngredient != null && currentIngredient == spawnedObject)
        {
            Destroy(currentIngredient); // Delete the ingredient (held object)
            AddValue(1); // Increment stash value
            currentIngredient = null; // Clear the reference
            collisionTimer = 0f; // Reset the collision timer
            Debug.Log("Held object deleted and stash value incremented.");
        }
        else
        {
            Debug.LogWarning("No valid ingredient to delete or incorrect object detected.");
        }
    }

    void OnCollisionEnter(Collision collision)
    {
        // Ensure the held object is colliding with the stash and is the spawned object
        if (interactionManager.isHolding && collision.gameObject == interactionManager.pickedObject)
        {
            if (collision.gameObject == spawnedObject) // Check if this is the spawned object
            {
                currentIngredient = collision.gameObject; // Assign the colliding object as the held ingredient
                Debug.Log("Ingredient detected and ready for deletion.");
            }
        }
    }

    void OnCollisionExit(Collision collision)
    {
        // Ensure the held object stops colliding with the stash
        if (collision.gameObject == currentIngredient)
        {
            currentIngredient = null; // Clear the reference
            collisionTimer = 0f; // Reset the timer
            Debug.Log("Ingredient no longer colliding.");
        }
    }
}

