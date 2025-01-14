using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[Serializable]
public class recipe
{
    public Ingredient FirstIngredient;
    public Ingredient SecondIngredient;
    public Ingredient ThirdIngredient;
}

public class MakePotion : MonoBehaviour
{
    [Tooltip("Dont change this! use list below!")]
    public Ingredient Ingredient1;
    [Tooltip("Dont change this! use list below!")]
    public Ingredient Ingredient2;
    [Tooltip("Dont change this! use list below!")]
    public Ingredient Ingredient3;

    [Tooltip("List containing all the recipes. change this.")]
    public List<recipe> potions;
}
