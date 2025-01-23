using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.UI;
using UnityEngine.Video;
using Unity.VisualScripting;
using UnityEngine.ProBuilder.MeshOperations;

public class DialogueSystem : MonoBehaviour
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

    private bool isDialogueActive = false;
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
            currentDialogueIndex = dialogueIndex;
            currentLineIndex = 0;
            isDialogueActive = true;

            if (dialogueUI != null)
                dialogueUI.SetActive(true);

            if (backgroundMusic != null && dialogueMusic != null)
            {
                backgroundMusic.clip = dialogueMusic;
                backgroundMusic.Play();
            }

            // Manage background visibility
            foreach (var bg in backgrounds)
            {
                if (bg != null)
                    bg.SetActive(false); // Deactivate all backgrounds
            }

            GameObject dialogueBackground = dialogues[currentDialogueIndex].backgroundGameObject;
            if (dialogueBackground != null)
                dialogueBackground.SetActive(true); // Activate the background for this dialogue

            // Play video if specified
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
    }

    void LoadDialogue()
    {
        Dialogue currentDialogue = dialogues[currentDialogueIndex];

        if (characterNameText != null)
            characterNameText.text = currentDialogue.characterName;

        ShowNextLine();
    }

    void ShowNextLine()
    {
        Dialogue currentDialogue = dialogues[currentDialogueIndex];

        if (currentLineIndex < currentDialogue.lines.Length)
        {
            Line currentLine = currentDialogue.lines[currentLineIndex];

            if (currentLine.characterSprite != null && characterImage != null)
            {
                characterImage.sprite = currentLine.characterSprite;
            }

            if (dialogueText != null)
            {
                if (typingCoroutine != null)
                    StopCoroutine(typingCoroutine);

                typingCoroutine = StartCoroutine(TypeText(currentLine.text));
            }

            currentLineIndex++;
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
    }

    





}
