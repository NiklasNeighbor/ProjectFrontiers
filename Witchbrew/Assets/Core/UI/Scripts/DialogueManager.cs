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
    public AudioSource dialogueAudioSource; 

    [Header("Backgrounds")]
    public GameObject[] backgrounds; // Array of background GameObjects

    public GameObject endingObject;

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
        public GameObject specificGameObject; // gameobject that should be activated for this dialogue
        public bool keepActiveAfterDialogue;
        public AudioClip dialogueSound;
    }

    public Dialogue[] dialogues;
    private int currentDialogueIndex = 0;
    private int currentLineIndex = 0;

    public bool isDialogueActive = false;
    private bool isTyping = false;
    private Coroutine typingCoroutine;

    public VideoPlayer videoPlayer;
    public Timer timer; // Reference to the Timer script

    private GameObject activeGameObject; // Stores the currently active GameObject

    void Start()
    {
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
            timer.PauseTimer();
        }
        else
        {
            timer.ResumeTimer();
        }
    }

    public void StartDialogue(int dialogueIndex)
    {
        if (dialogueIndex >= 0 && dialogueIndex < dialogues.Length)
        {
            StopAllCoroutines();
            currentDialogueIndex = dialogueIndex;
            currentLineIndex = 0;
            isDialogueActive = true;
            isTyping = false;

            if (dialogueUI != null)
                dialogueUI.SetActive(true);

            // Hide all previous backgrounds
            foreach (var bg in backgrounds)
            {
                if (bg != null)
                    bg.SetActive(false);
            }

            // Activate the background for this dialogue
            GameObject dialogueBackground = dialogues[currentDialogueIndex].backgroundGameObject;
            if (dialogueBackground != null)
                dialogueBackground.SetActive(true);

            // Deactivate previously active GameObject
            if (activeGameObject != null)
                activeGameObject.SetActive(false);

            // Activate specific GameObject for this dialogue
            activeGameObject = dialogues[currentDialogueIndex].specificGameObject;
            if (activeGameObject != null)
                activeGameObject.SetActive(true);

            if (dialogueAudioSource != null && dialogues[currentDialogueIndex].dialogueSound != null)
            {
                dialogueAudioSource.Stop(); // Stop any previous sound
                dialogueAudioSource.clip = dialogues[currentDialogueIndex].dialogueSound;
                dialogueAudioSource.Play();
            }

            // Play video if available
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

            LoadDialogue();
        }

        ShowNextLine();
    }

    public void EndDialogue()
    {
        isDialogueActive = false;

        if (dialogueUI != null)
            dialogueUI.SetActive(false);

        if (dialogueAudioSource != null)
        {
            dialogueAudioSource.Stop();
        }

        if (videoPlayer != null)
        {
            videoPlayer.Stop();
            videoPlayer.isLooping = false;
        }

        foreach (var bg in backgrounds)
        {
            if (bg != null)
                bg.SetActive(false);
        }

        if (activeGameObject != null && !dialogues[currentDialogueIndex].keepActiveAfterDialogue)
        {
            activeGameObject.SetActive(false);
            activeGameObject = null;
        }

        if (currentDialogueIndex == 1 && endingObject != null) // Good ending
        {
            endingObject.SetActive(true);
        }
        else if (currentDialogueIndex == 2 && endingObject != null) // Bad ending
        {
            endingObject.SetActive(true);
        }

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

        DisplayCurrentLine();
    }

    public void ShowNextLine()
    {
        currentLineIndex++;
        DisplayCurrentLine();
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
        yield return new WaitForSeconds(1f);
        if (Input.GetKey(nextKey))
        {
            ShowNextLine();
        }
    }

    public void StopTypingCoroutine()
    {
        if (typingCoroutine != null)
        {
            StopCoroutine(typingCoroutine);
            typingCoroutine = null;
        }

        if (dialogueText != null)
        {
            dialogueText.text = "";
        }

        isTyping = false;
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

