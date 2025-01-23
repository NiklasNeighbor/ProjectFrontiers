using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ResourceSpawner : MonoBehaviour
{
    [Tooltip("THIS IS NOT FOR MONOINGREDIENTS. \n Make a separate prefab with the WildResource script.")]
    public GameObject WildResourceToSpawn;
    [Tooltip("To what stash the resource will go")]
    public StashHandler StashHandler;
    public InteractionManager InteractionManager;

    public Vector3 SpawnRange;
    public int MaxAmount;
    public float RespawnDelay;
    private GameObject[] spawnList;
    private float timeStamp = 0;


    // Start is called before the first frame update
    void Start()
    {
        spawnList = new GameObject[MaxAmount];
        
    }

    // Update is called once per frame
    void Update()
    {
        CheckSpawn();
    }

    GameObject SpawnResource()
    {
        GameObject lastObject = Instantiate(WildResourceToSpawn, GetRandomPosition(), Quaternion.identity);
        WildResource wildResource = lastObject.GetComponent<WildResource>();
        wildResource.Destination = StashHandler;
        wildResource.InteractionManager = InteractionManager;
        return lastObject;
    }

    Vector3 GetRandomPosition()
    {
        float x = Random.Range(-SpawnRange.x / 2, SpawnRange.x / 2);
        float y = 0;
        float z = Random.Range(-SpawnRange.z / 2, SpawnRange.z / 2);
        return new Vector3(x, y, z) + transform.position;
    }

    void CheckSpawn()
    {
        // Only proceed if the respawn delay has passed
        if (Time.time >= timeStamp + RespawnDelay)
        {
            for (int i = 0; i < spawnList.Length; i++)
            {
                // Find an empty slot in the spawn list
                if (spawnList[i] == null)
                {
                    // Spawn a resource and assign it to the slot
                    spawnList[i] = SpawnResource();
                    Debug.Log("Spawned wild resource");

                    // Update timestamp after spawning
                    timeStamp = Time.time;
                    return; // Exit the method after spawning one resource
                }
            }

            // If we reach here, all slots are full
            Debug.Log("All slots are full, cannot spawn more resources");
            timeStamp = Time.time; // Update timestamp even if we couldn't spawn
        }
    }













    private void OnDrawGizmosSelected()
    {
        Gizmos.DrawWireCube(transform.position, SpawnRange);
    }

    
}
