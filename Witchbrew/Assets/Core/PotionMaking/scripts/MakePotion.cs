using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[Serializable]
public class recipe
{
    public string RecipeName;

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



    void Update()
    {
        if(HasIngredients())
        {
            CheckRecipes();
        }
    }

    public bool HasIngredients()
    {
        if (Ingredient1 != null && Ingredient2 != null && Ingredient3 != null)
        {
            return true;
        }
        return false;
    }

    public void CheckRecipes()
    {
        foreach (recipe recipe in potions)
        {
            bool HasIngredient1;
            bool HasIngredient2;
            bool HasIngredient3;

            //TODO: check recipe to see if it has each ingredient
        }
    }
}
