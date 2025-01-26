using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.UI;
using UnityEngine.Video;
using Unity.VisualScripting;
using UnityEngine.ProBuilder.MeshOperations;

public class DialogueManager : MonoBehaviour
{
    [Header("UI References")]
    public GameObject dialogueUI;
    public TMP_Text dialogueText;
    public TMP_Text characterNameText;
    public Image characterImage;

    [Header("Settings")]
    public KeyCode nextKey = KeyCode.Space;
    public float typingSpeed = 0.05f;

    [Header("Audio References")]
    public AudioSource backgroundMusic;
    public AudioClip dialogueMusic;
    public AudioClip defaultMusic;

    [Header("Backgrounds")]
    public GameObject[] backgrounds; // Array of background GameObjects

    [System.Serializable]
    public class Line
    {
        public string text;
        public Sprite characterSprite;
    }

    [System.Serializable]
    public class Dialogue
    {
        public string characterName;
        public Line[] lines;
        public VideoClip videoClip;
        public GameObject backgroundGameObject; // Background GameObject for this dialogue
    }

    public Dialogue[] dialogues;
    private int currentDialogueIndex = 0;
    private int currentLineIndex = 0;

    public bool isDialogueActive = false;
    private bool isTyping = false;
    private Coroutine typingCoroutine;

    public VideoPlayer videoPlayer;

    public Timer timer; // Reference to the Timer script

    void Start()
    {
        // Ensure the UI starts hidden and backgrounds are deactivated
        if (dialogueUI != null)
            dialogueUI.SetActive(false);

        foreach (var bg in backgrounds)
        {
            if (bg != null)
                bg.SetActive(false);
        }

        timer = FindObjectOfType<Timer>();

        StartDialogue(0); // Optionally start with the first dialogue
    }

    void Update()
    {
        if (isDialogueActive && Input.GetKeyDown(nextKey) && !isTyping)
        {
            ShowNextLine();
        }

        if (isDialogueActive)
        {
            timer.PauseTimer();  // Pause timer during dialogue
        }
        else
        {
            timer.ResumeTimer();  // Resume timer once dialogue ends
        }
    }

    public void StartDialogue(int dialogueIndex)
    {
        if (dialogueIndex >= 0 && dialogueIndex < dialogues.Length)
        {
            StopAllCoroutines();
            currentDialogueIndex = dialogueIndex;
            currentLineIndex = 0; // Reset to 0
            isDialogueActive = true;
            isTyping = false;

            if (dialogueUI != null)
                dialogueUI.SetActive(true);
            LoadDialogue(); // Call LoadDialogue instead of ShowNextLine
        }

        if (dialogueIndex >= 0 && dialogueIndex < dialogues.Length)
        {
            currentDialogueIndex = dialogueIndex;
            currentLineIndex = 0;
            isDialogueActive = true;

            // Stop any ongoing typing before starting the new dialogue
            StopTypingCoroutine();

            // Show the dialogue UI
            if (dialogueUI != null)
                dialogueUI.SetActive(true);

            // Update background and play music if available
            foreach (var bg in backgrounds)
            {
                if (bg != null)
                    bg.SetActive(false); // Hide all backgrounds initially
            }

            GameObject dialogueBackground = dialogues[currentDialogueIndex].backgroundGameObject;
            if (dialogueBackground != null)
                dialogueBackground.SetActive(true); // Show the background for this dialogue

            // Play the video if specified
            VideoClip videoClip = dialogues[currentDialogueIndex].videoClip;
            if (videoPlayer != null)
            {
                if (videoClip != null)
                {
                    videoPlayer.clip = videoClip;
                    videoPlayer.isLooping = true;
                    videoPlayer.Play();
                }
                else
                {
                    videoPlayer.Stop();
                }
            }

            // Start displaying the dialogue lines
            LoadDialogue();
        }

        ShowNextLine(); // Start with the first line
    }

    public void EndDialogue()
    {
        isDialogueActive = false;

        if (dialogueUI != null)
            dialogueUI.SetActive(false);

        if (backgroundMusic != null && defaultMusic != null)
        {
            backgroundMusic.clip = defaultMusic;
            backgroundMusic.Play();
        }

        if (videoPlayer != null)
        {
            videoPlayer.Stop();
            videoPlayer.isLooping = false;
        }

        foreach (var bg in backgrounds)
        {
            if (bg != null)
                bg.SetActive(false); // Deactivate all backgrounds when dialogue ends
        }

        // Resume timer after ending the dialogue
        if (timer != null)
        {
            timer.ResumeTimer();
        }
    }

    void LoadDialogue()
    {
        Dialogue currentDialogue = dialogues[currentDialogueIndex];

        if (characterNameText != null)
            characterNameText.text = currentDialogue.characterName;

        DisplayCurrentLine(); // Call new method to display the current line
    }


    public void ShowNextLine()
    {
        currentLineIndex++; // Increment the line index first
        DisplayCurrentLine(); // Then display the current (next) line
    }


    void DisplayCurrentLine()
    {
        Dialogue currentDialogue = dialogues[currentDialogueIndex];

        if (currentLineIndex < currentDialogue.lines.Length)
        {
            Line currentLine = currentDialogue.lines[currentLineIndex];

            if (currentLine.characterSprite != null && characterImage != null)
            {
                characterImage.sprite = currentLine.characterSprite;
            }

            StopTypingCoroutine();
            typingCoroutine = StartCoroutine(TypeText(currentLine.text));
        }
        else
        {
            EndDialogue();
        }
    }



    IEnumerator TypeText(string line)
    {
        isTyping = true;
        dialogueText.text = "";

        foreach (char c in line)
        {
            dialogueText.text += c;
            yield return new WaitForSeconds(typingSpeed);
        }

        isTyping = false;
        // Signal that typing is complete, allowing for automatic progression if needed
        yield return new WaitForSeconds(1f); // Optional: Wait a bit after typing finishes
        if (Input.GetKey(nextKey))
        {
            ShowNextLine();
        }
    }


    public void StopTypingCoroutine()
    {
        if (typingCoroutine != null)
        {
            StopCoroutine(typingCoroutine); // Stop any active typing coroutine
            typingCoroutine = null;         // Clear the reference
        }

        if (dialogueText != null)
        {
            dialogueText.text = ""; // Clear the dialogue text field
        }

        isTyping = false; // Ensure the typing flag is reset
    }

    public GameObject GoodEndVideo;
    public GameObject BadEndVideo;
    public void CheckGameEnd()
    {
        Timer timer = FindObjectOfType<Timer>();
        if (timer != null)
        {
            timer.PauseTimer();
            timer.enabled = false;
        }

        float coinThreshold = 400;
        int dialogueIndex;
        if (FindObjectOfType<Orders>().TotalCoins >= coinThreshold)
        {
            dialogueIndex = 1;
            if (GoodEndVideo != null) GoodEndVideo.SetActive(true);
        }
        else
        {
            dialogueIndex = 2;
            if (BadEndVideo != null) BadEndVideo.SetActive(true);
        }

        StopAllCoroutines();
        if (dialogueUI != null)
            dialogueUI.SetActive(true);

        StartDialogue(dialogueIndex);
    }

}
