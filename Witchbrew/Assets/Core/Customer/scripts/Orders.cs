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
    public TMP_Text SpeechBubbleDisplay;
    public TMP_Text CoinPopup;
    public TMP_Text TimePopup;

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


    public List<AudioClip> CustomerSounds;



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
        CurrentCustomer = Instantiate(NextCustomer, CustomerSpawnLocation.position, CustomerSpawnLocation.rotation);
    }

    public void UpdateTip()
    {
        if (RequestedPotion != null)
        {
            TipAmount = MaxTip - (Time.time - OrderTimestamp);
            TipAmount = Mathf.Clamp(TipAmount, 0, MaxTip);
        }
    }

    private IEnumerator ShowCoinPopup(float amountGained)
    {
        if (CoinPopup != null)
        {
            // Set the text to show the amount gained with a plus sign
            CoinPopup.text = "+" + amountGained.ToString();

            // Enable the CoinPopup
            CoinPopup.gameObject.SetActive(true);

            // Wait for 2 seconds
            yield return new WaitForSeconds(2f);

            // Disable the CoinPopup
            CoinPopup.gameObject.SetActive(false);
        }
    }

    private IEnumerator ShowTimePopup(float timeIncrease)
    {
        if (TimePopup != null)
        {
            // Set the text to show the time increase with a plus sign
            TimePopup.text = "+" + timeIncrease.ToString() + " seconds";

            // Enable the TimePopup
            TimePopup.gameObject.SetActive(true);

            // Wait for 2 seconds
            yield return new WaitForSeconds(2f);

            // Disable the TimePopup
            TimePopup.gameObject.SetActive(false);
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
                float totalGained = PotionPrice + TipAmount; // Calculate the total amount gained
                TotalCoins = TotalCoins + totalGained;
                CoinDisplay.text = TotalCoins.ToString();

                if (correctPotionSFX != null && audioSource != null)
                {
                    audioSource.PlayOneShot(correctPotionSFX);
                }

                StartCoroutine(ShowCoinPopup(totalGained));

                if (timer != null)
                {
                    StartCoroutine(ShowTimePopup(timer.timeIncreaseAmount));
                }
            }
            else
            {
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