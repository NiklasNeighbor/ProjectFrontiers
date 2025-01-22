using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Rigidbody))]
[RequireComponent(typeof(Collider))]
public class MonoIngredient : MonoBehaviour
{

    public Ingredient Ingredient;
    public void Start()
    {
        gameObject.tag = "Pickup";
    }

}
