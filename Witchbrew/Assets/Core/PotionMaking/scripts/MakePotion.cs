using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[Serializable]
public class recipe
{
    public string RecipeName;
    public GameObject PotionProduct;

    public Ingredient FirstIngredient;
    public Ingredient SecondIngredient;
    public Ingredient ThirdIngredient;
}

public class MakePotion : MonoBehaviour
{
    [Tooltip("The Ingredients that were already added into the pot")]
    public Ingredient[] ingredients = new Ingredient[3];

    [Tooltip("List containing all the recipes. change this.")]
    public List<recipe> potions;

    [Tooltip("With how much force the potion is launched out of the cauldron upon completion")]
    public float ExpelForce = 200f;



    void Update()
    {
        if(HasIngredients())
        {
            CheckRecipes();
        }
    }

    public bool HasIngredients()
    {
        if (ingredients[0].type != Ingredient.ingredient.None && ingredients[1].type != Ingredient.ingredient.None && ingredients[2].type != Ingredient.ingredient.None)
        {
            return true;
        }
        return false;
    }

    public void CheckRecipes()
    {
        foreach (recipe recipe in potions)
        {
            bool HasIngredient1 = false;
            bool HasIngredient2 = false;
            bool HasIngredient3 = false;

            if (CheckForIngredient(recipe.FirstIngredient) && !HasIngredient1)
            {
                HasIngredient1 = true;
            }

            if (CheckForIngredient(recipe.SecondIngredient) && !HasIngredient2)
            {
                HasIngredient2 = true;
            }

            if (CheckForIngredient(recipe.ThirdIngredient) && !HasIngredient3)
            {
                HasIngredient3 = true;
            }

            //What happens when the ingredients were correct
            if(HasIngredient1 && HasIngredient2 && HasIngredient3)
            {

                Debug.Log("created potion: " + recipe.RecipeName);
                Debug.Log("Ingredients: " + ingredients[0].prep + " " + ingredients[0].type + ", " + ingredients[1].prep + " " + ingredients[1].type + ", " + ingredients[2].prep + " " + ingredients[2].type);

                PotionSuccess(recipe);

                ingredients[0].type = Ingredient.ingredient.None;
                ingredients[1].type = Ingredient.ingredient.None;
                ingredients[2].type = Ingredient.ingredient.None;
                return;
            }
        }
        PotionFail();

        ingredients[0].type = Ingredient.ingredient.None;
        ingredients[1].type = Ingredient.ingredient.None;
        ingredients[2].type = Ingredient.ingredient.None;
    }

    public bool CheckForIngredient(Ingredient RightIngredient)
    {
        foreach(Ingredient ingredient in ingredients)
        {
            if ((ingredient.type == RightIngredient.type) && (ingredient.prep == RightIngredient.prep))
            {
                
                return true;
            }
        }
        return false;
    }

    public void PotionSuccess(recipe FinalPotion)
    {
        //Insert code for successful recipe here
        if (FinalPotion.PotionProduct != null)
        {
            //spawn the potion and get references
            GameObject Product = Instantiate(FinalPotion.PotionProduct, gameObject.transform.position + Vector3.up, Quaternion.identity);
            Rigidbody rb = Product.GetComponent<Rigidbody>();
            MonoPotion ProductRecipe = rb.GetComponent<MonoPotion>();
            ProductRecipe.recipe = FinalPotion;

            //add some fun physics
            float x = UnityEngine.Random.Range(-50f, 50f);
            float z = UnityEngine.Random.Range(-50f, 50f);
            rb.AddForce(new Vector3(x, ExpelForce, z));
            x = UnityEngine.Random.Range(-20f, 20f);
            z = UnityEngine.Random.Range(-20f, 20f);
            float y = UnityEngine.Random.Range(-20f, 20f);
            rb.AddTorque(new Vector3(x, y, z));

        }
    }

    public void PotionFail()
    {
        Debug.Log("No Potion with those ingredients found!");

        //Insert code for failed recipe here
    }



    public void OnTriggerEnter(Collider other)
    {
        MonoIngredient NewIngredient = other.gameObject.GetComponent<MonoIngredient>();
        
        if (NewIngredient != null)
        {
            for (int i = 0; i < ingredients.Length; i++)
            {
                if (ingredients[i].type == Ingredient.ingredient.None)
                {
                    Debug.Log("Added ingredient to: " + ingredients[i]);
                    ingredients[i] = NewIngredient.Ingredient;
                    Destroy(other.gameObject);
                    return;
                }
            }
        }
    }
}
