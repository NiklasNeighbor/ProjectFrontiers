using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class RecipeBookManager : MonoBehaviour
{
    [Header("UI References")]
    public GameObject recipeBookUI; // The parent UI GameObject for the recipe book
    public Image pageImage; // Image component to display the current page content
    public TMP_Text pageNumberText; // Text to display the current page number
    public Button nextPageButton; // Button to go to the next page
    public Button previousPageButton; // Button to go to the previous page

    [Header("Page Button Collections")]
    public GameObject[] pageButtonCollections; // Array of button collections for each page (e.g., PotionButtons1, PotionButtons2, etc.)

    [Header("Settings")]
    public KeyCode toggleMenuKey = KeyCode.Tab; // Key to open/close the recipe book
    public Sprite[] pageSprites; // Array of sprites for each page
    private int currentPage = 0; // Tracks the current page index

    public bool isRecipeBookOpen = false; // Tracks whether the recipe book is open

    void Start()
    {
        // Ensure UI starts hidden
        if (recipeBookUI != null)
            recipeBookUI.SetActive(false);

        // Attach button listeners
        if (nextPageButton != null)
            nextPageButton.onClick.AddListener(NextPage);

        if (previousPageButton != null)
            previousPageButton.onClick.AddListener(PreviousPage);

        UpdatePage(); // Update UI to show the initial page
    }

    void Update()
    {
        // Toggle the recipe book when the toggle key is pressed
        if (Input.GetKeyDown(toggleMenuKey))
        {
            ToggleRecipeBook();
        }
    }

    void ToggleRecipeBook()
    {
        isRecipeBookOpen = !isRecipeBookOpen;
        if (recipeBookUI != null)
        {
            recipeBookUI.SetActive(isRecipeBookOpen);

            // Lock cursor and disable player camera movement if open
            Cursor.lockState = isRecipeBookOpen ? CursorLockMode.None : CursorLockMode.Locked;
            Cursor.visible = isRecipeBookOpen;
        }
    }

    void NextPage()
    {
        // Go to the next page if it exists
        if (currentPage < pageSprites.Length - 1)
        {
            currentPage++;
            UpdatePage();
        }
    }

    void PreviousPage()
    {
        // Go to the previous page if it exists
        if (currentPage > 0)
        {
            currentPage--;
            UpdatePage();
        }
    }

    void UpdatePage()
    {
        // Update the page content and page number
        if (pageSprites != null && pageSprites.Length > 0 && pageImage != null)
        {
            pageImage.sprite = pageSprites[currentPage]; // Update the sprite for the current page
        }

        if (pageNumberText != null)
        {
            pageNumberText.text = $"Page {currentPage + 1}/{pageSprites.Length}"; // Update the page number text
        }

        // Enable/disable navigation buttons based on the current page
        if (previousPageButton != null)
            previousPageButton.interactable = currentPage > 0;

        if (nextPageButton != null)
            nextPageButton.interactable = currentPage < pageSprites.Length - 1;

        // Activate/deactivate page-specific button collections
        HandlePageButtonVisibility();
    }

    void HandlePageButtonVisibility()
    {
        // Hide all button collections first
        foreach (var buttonCollection in pageButtonCollections)
        {
            if (buttonCollection != null)
                buttonCollection.SetActive(false); // Deactivate all button collections
        }

        // Now, activate the button collection for the current page
        if (currentPage >= 0 && currentPage < pageButtonCollections.Length)
        {
            if (pageButtonCollections[currentPage] != null)
                pageButtonCollections[currentPage].SetActive(true); // Activate the button collection for the current page
        }
    }

    // Helper method to check if recipe book is open
    public bool IsRecipeBookOpen()
    {
        return isRecipeBookOpen;
    }
}
