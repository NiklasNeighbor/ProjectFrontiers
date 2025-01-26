using UnityEngine;
using TMPro;

public class Timer : MonoBehaviour
{
    [Header("UI References")]
    public TMP_Text timerText; // Reference to the UI Text component
    public float startTime = 240f; // 4 minutes in seconds (240 seconds)

    [Header("Popup Reference")]
    public GameObject TutorialPopUp; // Reference to the TutorialPopUp GameObject

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

            // Format the remaining time as minutes and seconds
            int minutes = Mathf.FloorToInt(timeRemaining / 60);
            int seconds = Mathf.FloorToInt(timeRemaining % 60);

            // Update the UI text to show the formatted time
            timerText.text = string.Format("Time left: {0:00}:{1:00}", minutes, seconds);
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






}

