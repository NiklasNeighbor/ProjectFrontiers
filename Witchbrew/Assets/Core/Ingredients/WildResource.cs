using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Rigidbody))]
[RequireComponent(typeof(Collider))]
public class WildResource : MonoBehaviour
{

    public StashHandler Destination;
    public InteractionManager InteractionManager;

    // Start is called before the first frame update
    void Start()
    {
        gameObject.tag = "Pickup";
    }

    // Update is called once per frame
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
                Destination.stashValue = Destination.stashValue + 1;
                Destroy(gameObject);
            }
        }
    }

}
