using System.Collections;
using System.Collections.Generic;
using UnityEngine;


[RequireComponent(typeof(Rigidbody))]
[RequireComponent(typeof(Collider))]
public class MonoPotion : MonoBehaviour
{
    
    public recipe recipe;

    

    public void Start()
    {
        gameObject.tag = "pickup";
    }
}
