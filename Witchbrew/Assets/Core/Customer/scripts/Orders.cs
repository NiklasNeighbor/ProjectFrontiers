using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.UI;
using System.Runtime.CompilerServices;

public class Orders : MonoBehaviour
{
    public recipe RequestedPotion;
    public MakePotion RecipeBook;

    public TMP_Text OrderDisplay;
    public RawImage OrderImage;
    public TMP_Text CoinDisplay;
    public TMP_Text SpeechBubbleDisplay; // New TMP_Text for customer questions

    public float OrderTimestamp;
    public float TipAmount;
    public float TotalCoins;

    [Space(20)]
    public float MaxTip = 80;
    public float PotionPrice = 100;

    [Header("SFX Settings")]
    public AudioClip correctPotionSFX; // Sound effect for correct potion
    public AudioClip wrongPotionSFX;   // Sound effect for wrong potion

    public AudioSource audioSource; // Reference to the existing AudioSource

    private Timer timer; // Reference to the Timer script

    [Space(20)]
    [Header("Customer Prefab Settings")]
    [Tooltip("The location at which the customer spawns. Create an empty GameObject to put in here.")]
    public Transform CustomerSpawnLocation;
    [Tooltip("A list of all the possible Customer Prefabs that can be spawned")]
    public List<GameObject> CustomerPrefabs;
    [Tooltip("The currently active customer. Doesn't need to be touched.")]
    public GameObject CurrentCustomer;

    // Start is called before the first frame update
    void Start()
    {
        GetRandomRecipe();
        CoinDisplay.text = TotalCoins.ToString();

        // Find the Timer script in the scene
        timer = FindObjectOfType<Timer>();

        // Ensure the AudioSource is assigned
        if (audioSource == null)
        {
            Debug.LogWarning("AudioSource not assigned in the Inspector.");
        }
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
            if (OrderDisplay != null)
            {
                OrderDisplay.text = "Order:\n" + RequestedPotion.RecipeName;
            }
            if (OrderImage != null)
            {
                OrderImage.texture = RequestedPotion.OrderTexture;
            }
            if (SpeechBubbleDisplay != null && RequestedPotion.questions.Count > 0)
            {
                int questionIndex = Random.Range(0, RequestedPotion.questions.Count);
                SpeechBubbleDisplay.text = RequestedPotion.questions[questionIndex];
            }

            OrderTimestamp = Time.time;
            TipAmount = MaxTip;
            SpawnRandomCustomer();

        }
    }

    public void SpawnRandomCustomer()
    {
        
        if (CurrentCustomer != null)
        {
            Destroy(CurrentCustomer);
        }
        GameObject NextCustomer = CustomerPrefabs[Random.Range(0, CustomerPrefabs.Count)];
        CurrentCustomer = Instantiate(NextCustomer, CustomerSpawnLocation.position, Quaternion.identity);
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
            if (PresentedPotion.recipe == RequestedPotion)
            {
                TipAmount = Mathf.Round(TipAmount);
                TotalCoins = TotalCoins + PotionPrice + TipAmount;
                CoinDisplay.text = TotalCoins.ToString();

                // Increase time if the potion is correct
                if (timer != null)
                {
                    timer.IncreaseTime(timer.timeIncreaseAmount);
                }

                // Play the correct potion sound effect
                if (correctPotionSFX != null && audioSource != null)
                {
                    audioSource.PlayOneShot(correctPotionSFX);
                }
            }
            else
            {
                // Play the wrong potion sound effect
                if (wrongPotionSFX != null && audioSource != null)
                {
                    audioSource.PlayOneShot(wrongPotionSFX);
                }
            }
            Destroy(other.gameObject);
            GetRandomRecipe();
        }
    }
}