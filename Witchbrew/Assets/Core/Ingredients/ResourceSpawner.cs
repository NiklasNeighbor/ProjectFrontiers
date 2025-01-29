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
        GameObject lastObject = Instantiate(WildResourceToSpawn, GetRandomPosition(), transform.rotation);
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
        if (timeStamp + RespawnDelay < Time.time)
        {
            for (int i = 0; i < spawnList.Length; i++)
            {
                if (spawnList[i] == null)
                {
                    spawnList[i] = SpawnResource();
                    timeStamp = Time.time;
                    Debug.Log("Spawned wild resource");
                    return;
                }
            }
            timeStamp = Time.time;
        }
    }


    private void OnDrawGizmosSelected()
    {
        Gizmos.DrawWireCube(transform.position, SpawnRange);
    }


}   