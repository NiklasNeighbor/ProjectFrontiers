using UnityEngine;
using UnityEngine.UI; // Required for UI Image
using TMPro;

public class Timer : MonoBehaviour
{
    [Header("UI References")]
    public TMP_Text timerText; // Reference to the UI Text component
    public Image fillImage;    // Reference to the fill UI Image (behind the hourglass)
    public float startTime = 240f; // 4 minutes in seconds (240 seconds)

    [Header("Popup Reference")]
    public GameObject TutorialPopUp; // Reference to the TutorialPopUp GameObject

    [Header("Time Increase Settings")]
    public bool enableTimeIncrease = true; // Toggle to enable/disable time increase
    public float timeIncreaseAmount = 20f; // Amount of time to add when a potion is correct

    private float timeRemaining; // The time left on the timer
    private bool isTimerRunning = false;

    void Start()
    {
        timeRemaining = startTime;  // Initialize the timer with 4 minutes
        isTimerRunning = true;      // Start the timer
    }

    void Update()
    {
        if (TutorialPopUp != null && TutorialPopUp.activeSelf)
        {
            PauseTimer(); // Pause the timer if the popup is active
        }
        else
        {
            ResumeTimer(); // Resume the timer if the popup is not active
        }

        if (isTimerRunning)
        {
            // Decrease the timer by time passed in each frame
            timeRemaining -= Time.deltaTime;

            // Ensure the timer doesn't go below 0
            if (timeRemaining <= 0)
            {
                timeRemaining = 0;
                isTimerRunning = false;  // Stop the timer once it reaches 0

                // Call the game-ending logic
                GameEnd();
            }

            // Update the fill amount based on the remaining time
            UpdateFillAmount();

            // Format the remaining time as minutes and seconds
            int minutes = Mathf.FloorToInt(timeRemaining / 60);
            int seconds = Mathf.FloorToInt(timeRemaining % 60);

            // Update the UI text to show the formatted time
            timerText.text = string.Format("{0:00}:{1:00}", minutes, seconds);
        }
    }

    private void UpdateFillAmount()
    {
        if (fillImage != null)
        {
            // Calculate the fill amount (normalized value between 0 and 1)
            float fillAmount = timeRemaining / startTime;
            fillImage.fillAmount = fillAmount;
        }
    }

    public void PauseTimer()
    {
        isTimerRunning = false;
    }

    public void ResetTimer()
    {
        timeRemaining = startTime;
        isTimerRunning = true;
        UpdateFillAmount(); // Reset the fill amount
    }

    public void ResumeTimer()
    {
        isTimerRunning = true;
    }

    public void GameEnd()
    {
        PauseTimer();
        DialogueManager dialogueManager = FindObjectOfType<DialogueManager>();
        if (dialogueManager != null)
        {
            dialogueManager.CheckGameEnd();
            enabled = false; // Disable the Timer script completely
        }
    }

    // Method to increase time when a potion is correct
    public void IncreaseTime(float amount)
    {
        if (enableTimeIncrease)
        {
            timeRemaining += amount;
            // Ensure the timer doesn't exceed the start time
            if (timeRemaining > startTime)
            {
                timeRemaining = startTime;
            }
            UpdateFillAmount(); // Update the fill amount after increasing time
        }
    }
}