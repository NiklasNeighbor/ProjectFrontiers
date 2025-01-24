using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.UI;

public class Orders : MonoBehaviour
{
    public recipe RequestedPotion;
    public MakePotion RecipeBook;

    public TMP_Text OrderDisplay;
    public TMP_Text CoinDisplay;

    public float OrderTimestamp;
    public float TipAmount;
    public float TotalCoins;

    [Space(20)]
    public float MaxTip = 80;
    public float PotionPrice = 100;

    // Start is called before the first frame update
    void Start()
    {
        GetRandomRecipe();
        CoinDisplay.text = "Coins: " + TotalCoins.ToString();

    }

    // Update is called once per frame
    void Update()
    {
        UpdateTip();
    }

    public void GetRandomRecipe()
    {
        if (RecipeBook != null)
        {
            int RandomIndex = Random.Range(0, RecipeBook.potions.Count);
            RequestedPotion = RecipeBook.potions[RandomIndex];
            OrderDisplay.text = "Order:\n" + RequestedPotion.RecipeName;
            OrderTimestamp = Time.time;
            TipAmount = MaxTip;
        }
    }

    public void UpdateTip()
    {
        if (RequestedPotion != null)
        {
            TipAmount = MaxTip - (Time.time - OrderTimestamp);
            TipAmount = Mathf.Clamp(TipAmount, 0, MaxTip);
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        MonoPotion PresentedPotion = other.gameObject.GetComponent<MonoPotion>();
        if (PresentedPotion != null)
        {
            if(PresentedPotion.recipe == RequestedPotion)
            {
                TipAmount = Mathf.Round(TipAmount);
                TotalCoins = TotalCoins + PotionPrice + TipAmount;
                CoinDisplay.text = "Coins: " + TotalCoins.ToString();
                
            }
            Destroy(other.gameObject);
            GetRandomRecipe();
        }

    }
}
