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
    public ProcessorType processorType;

    [System.Serializable]
    public class IngredientTransformation
    {
        public GameObject ingredientPrefab;
        public GameObject slicedVersion;
        public GameObject roastedVersion;
        public GameObject grindedVersion;
    }

    [Header("Ingredient Transformations")]
    public List<IngredientTransformation> ingredientTransformations;

    public float spawnDelay = 2f;

    private bool isProcessing = false;

    [Header("Layers")]
    public string ingredientLayer = "Ingredient";
    public string processedLayer = "Processed";

    [Header("Progress Bar")]
    public Transform progressBar; // Reference to the progress bar object
    private Vector3 initialScale; // Initial scale of the progress bar

    [Header("Spawn Settings")]
    public Transform spawnLocation; // Reference to the location where the processed ingredient will be spawned

    [Header("VFX Prefab")]
    public GameObject vfxPrefab; 

    private void Start()
    {
        // Store the initial scale of the progress bar
        if (progressBar != null)
        {
            initialScale = progressBar.localScale;
        }
    }

    private void OnCollisionEnter(Collision collision)
    {
        if (isProcessing) return;

        if (collision.gameObject.layer != LayerMask.NameToLayer(ingredientLayer)) return;

        IngredientTransformation transformation = FindIngredientTransformation(collision.gameObject);

        if (transformation != null)
        {
            isProcessing = true;

            if (vfxPrefab != null)
            {
                Instantiate(vfxPrefab, collision.transform.position, collision.transform.rotation);
            }

            Destroy(collision.gameObject);
            StartCoroutine(SpawnProcessedVersion(transformation));
        }
    }

    private IngredientTransformation FindIngredientTransformation(GameObject ingredient)
    {
        foreach (IngredientTransformation transformation in ingredientTransformations)
        {
            if (ingredient != null && IsSamePrefab(ingredient, transformation.ingredientPrefab))
            {
                return transformation;
            }
        }
        return null;
    }

    private bool IsSamePrefab(GameObject ingredient, GameObject prefab)
    {
        return ingredient != null && ingredient.name.StartsWith(prefab.name);
    }

    private IEnumerator SpawnProcessedVersion(IngredientTransformation transformation)
    {
        // Initialize progress bar
        if (progressBar != null)
        {
            progressBar.gameObject.SetActive(true);
            progressBar.localScale = initialScale; // Reset to full size
        }

        float elapsedTime = 0f;

        // Update the progress bar during processing
        while (elapsedTime < spawnDelay)
        {
            elapsedTime += Time.deltaTime;

            if (progressBar != null)
            {
                float progress = elapsedTime / spawnDelay; // Progress from 0 to 1
                progressBar.localScale = new Vector3(
                    progressBar.localScale.x,
                    progressBar.localScale.y,
                    Mathf.Lerp(initialScale.z, 0, progress) // Decrease z-scale
                );
            }

            yield return null;
        }

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

        if (prefabToSpawn != null)
        {
            // Use the spawnLocation to instantiate the object at a set location
            Vector3 spawnPosition = spawnLocation != null ? spawnLocation.position : transform.position;
            GameObject processedObject = Instantiate(prefabToSpawn, spawnPosition, spawnLocation.rotation);
            processedObject.layer = LayerMask.NameToLayer(processedLayer);
        }

        // Hide progress bar after processing
        if (progressBar != null)
        {
            progressBar.gameObject.SetActive(false);
        }

        isProcessing = false;
    }
}
