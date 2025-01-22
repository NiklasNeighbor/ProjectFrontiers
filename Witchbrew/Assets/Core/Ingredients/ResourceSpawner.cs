using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ResourceSpawner : MonoBehaviour
{
    [Tooltip("THIS IS NOT FOR MONOINGREDIENTS")]
    public GameObject WildResourceToSpawn;
    [Tooltip("To what stash the resource will go")]
    public StashHandler StashHandler;

    public Vector3 SpawnRange;
    public int MaxAmount;
    public float RespawnDelay;


    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    private void OnDrawGizmosSelected()
    {
        Gizmos.DrawWireCube(transform.position, SpawnRange);
    }
}
