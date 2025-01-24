using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class IngredientProcessor : MonoBehaviour
{
    public enum ProcessorType
    {
        Slicer,
        Roaster,
        Grinder
    }

    [Header("Processor Settings")]
    public ProcessorType processorType; // Slicer, Roaster, Grinder

    [System.Serializable]
    public class IngredientTransformation
    {
        public GameObject ingredientPrefab; // The ingredient prefab (e.g., RedCube, BlueCube)
        public GameObject slicedVersion; // Sliced version prefab
        public GameObject roastedVersion; // Roasted version prefab
        public GameObject grindedVersion; // Grinded version prefab
    }

    [Header("Ingredient Transformations")]
    public List<IngredientTransformation> ingredientTransformations; // List of transformations

    public float spawnDelay = 2f; // Delay before spawning the processed version

    private bool isProcessing = false; // Lock to prevent multiple processes at the same time

    [Header("Layers")]
    public string ingredientLayer = "Ingredient"; // Layer for the original ingredient objects
    public string processedLayer = "Processed";  // Layer for the processed objects

    private void OnCollisionEnter(Collision collision)
    {
        // If the processor is already handling an object, ignore this collision
        if (isProcessing) return;

        // Check if the object is on the correct layer
        if (collision.gameObject.layer != LayerMask.NameToLayer(ingredientLayer)) return;

        // Check if the colliding object matches a known ingredient prefab
        IngredientTransformation transformation = FindIngredientTransformation(collision.gameObject);

        if (transformation != null)
        {
            // Set the lock to prevent further processing
            isProcessing = true;

            // Destroy the original object
            Destroy(collision.gameObject);

            // Start spawning the processed version
            StartCoroutine(SpawnProcessedVersion(transformation, collision.transform.position));
        }
    }

    private IngredientTransformation FindIngredientTransformation(GameObject ingredient)
    {
        // Check each transformation and compare the prefab with the collided object's prefab
        foreach (IngredientTransformation transformation in ingredientTransformations)
        {
            if (ingredient.CompareTag(transformation.ingredientPrefab.tag)) // Compare tags for prefab matching
            {
                return transformation;
            }
        }
        return null;
    }

    private IEnumerator SpawnProcessedVersion(IngredientTransformation transformation, Vector3 position)
    {
        // Wait for the delay
        yield return new WaitForSeconds(spawnDelay);

        // Determine which version to spawn based on the processor type
        GameObject prefabToSpawn = null;

        switch (processorType)
        {
            case ProcessorType.Slicer:
                prefabToSpawn = transformation.slicedVersion;
                break;
            case ProcessorType.Roaster:
                prefabToSpawn = transformation.roastedVersion;
                break;
            case ProcessorType.Grinder:
                prefabToSpawn = transformation.grindedVersion;
                break;
        }

        // Spawn the processed ingredient if a prefab is assigned
        if (prefabToSpawn != null)
        {
            GameObject processedObject = Instantiate(prefabToSpawn, position, Quaternion.identity);

            // Assign the "Processed" layer to the spawned object
            processedObject.layer = LayerMask.NameToLayer(processedLayer);
        }
        else
        {
            Debug.LogWarning($"No prefab assigned for {processorType} for ingredient {transformation.ingredientPrefab.name}");
        }

        // Release the lock to allow processing of the next ingredient
        isProcessing = false;
    }
}
