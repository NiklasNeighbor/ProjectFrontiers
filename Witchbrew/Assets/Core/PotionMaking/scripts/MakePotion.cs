using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

[Serializable]
public class recipe
{
    public string RecipeName;
    public Texture2D OrderTexture;
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

    public TMP_Text failMessageText;
    public TMP_Text ingredientText;

    [Header("VFX")]
    public GameObject failVFXPrefab;
    public Transform explosionLocation; // Reference to the location where the explosion will happen

    void Start()
    {
        UpdateIngredientText(); // Ensure UI starts at "0/3"
    }

    void Update()
    {
        if (HasIngredients())
        {
            CheckRecipes();
        }
    }

    public bool HasIngredients()
    {
        return (ingredients[0].type != Ingredient.ingredient.None &&
                ingredients[1].type != Ingredient.ingredient.None &&
                ingredients[2].type != Ingredient.ingredient.None);
    }

    public void CheckRecipes()
    {
        foreach (recipe recipe in potions)
        {
            bool HasIngredient1 = CheckForIngredient(recipe.FirstIngredient);
            bool HasIngredient2 = CheckForIngredient(recipe.SecondIngredient);
            bool HasIngredient3 = CheckForIngredient(recipe.ThirdIngredient);

            // If all three ingredients match a recipe
            if (HasIngredient1 && HasIngredient2 && HasIngredient3)
            {
                Debug.Log("Created potion: " + recipe.RecipeName);
                PotionSuccess(recipe);
                ResetIngredients();
                return;
            }
        }

        // If no recipe matched
        PotionFail();
        ResetIngredients();
    }

    public bool CheckForIngredient(Ingredient RightIngredient)
    {
        foreach (Ingredient ingredient in ingredients)
        {
            if (ingredient.type == RightIngredient.type && ingredient.prep == RightIngredient.prep)
            {
                return true;
            }
        }
        return false;
    }

    public void PotionSuccess(recipe FinalPotion)
    {
        if (FinalPotion.PotionProduct != null)
        {
            GameObject Product = Instantiate(FinalPotion.PotionProduct, transform.position + Vector3.up, Quaternion.identity);
            Rigidbody rb = Product.GetComponent<Rigidbody>();
            MonoPotion ProductRecipe = rb.GetComponent<MonoPotion>();
            ProductRecipe.recipe = FinalPotion;

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

        if (failVFXPrefab != null)
        {
            Vector3 spawnPosition = explosionLocation != null ? explosionLocation.position : transform.position;
            Quaternion spawnRotation = explosionLocation != null ? explosionLocation.rotation : Quaternion.identity;
            Instantiate(failVFXPrefab, spawnPosition, spawnRotation);
        }

        if (failMessageText != null)
        {
            StartCoroutine(DisplayFailMessage());
        }
    }

    private IEnumerator DisplayFailMessage()
    {
        failMessageText.text = "Wrong ingredients!";
        failMessageText.gameObject.SetActive(true);

        yield return new WaitForSeconds(3f);

        failMessageText.gameObject.SetActive(false);
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
                    ingredients[i] = NewIngredient.Ingredient;
                    Destroy(other.gameObject);
                    UpdateIngredientText(); // Update UI when ingredient is added
                    return;
                }
            }
        }
    }

    private void UpdateIngredientText()
    {
        int count = 0;
        foreach (var ingredient in ingredients)
        {
            if (ingredient.type != Ingredient.ingredient.None)
            {
                count++;
            }
        }
        ingredientText.text = $"{count}/3"; // Update UI
    }

    private void ResetIngredients()
    {
        for (int i = 0; i < ingredients.Length; i++)
        {
            ingredients[i].type = Ingredient.ingredient.None;
        }
        UpdateIngredientText(); // Reset UI to "0/3"
    }
}
