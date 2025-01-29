using UnityEngine;

public class FirstPersonLook : MonoBehaviour
{
    [SerializeField]
    Transform character;
    public RecipeBookManager RecipeBookManager;
    public GameObject TutorialPopUp; // Reference to the UI popup GameObject
    public GameObject WinScreen; // Reference to the Win Screen UI
    public GameObject LoseScreen; // Reference to the Lose Screen UI
    public float sensitivity = 2;
    public float smoothing = 1.5f;

    public float upperClamp = 90f; // Default values
    public float lowerClamp = -90f;

    Vector2 velocity;
    Vector2 frameVelocity;

    void Reset()
    {
        character = GetComponentInParent<FirstPersonMovement>().transform;
    }

    void Start()
    {
        Cursor.lockState = CursorLockMode.Locked;
    }

    void Update()
    {
        // Check if any UI element that should unlock the cursor is active
        bool shouldUnlockCursor =
            (TutorialPopUp != null && TutorialPopUp.activeSelf) ||
            (WinScreen != null && WinScreen.activeSelf) ||
            (LoseScreen != null && LoseScreen.activeSelf) ||
            RecipeBookManager.isRecipeBookOpen;

        if (shouldUnlockCursor)
        {
            Cursor.lockState = CursorLockMode.None;
            Cursor.visible = true;
            return; // Prevent camera movement
        }
        else
        {
            Cursor.lockState = CursorLockMode.Locked;
            Cursor.visible = false;
        }

        // Mouse look only works if the cursor is locked
        Vector2 mouseDelta = new Vector2(Input.GetAxisRaw("Mouse X"), Input.GetAxisRaw("Mouse Y"));
        Vector2 rawFrameVelocity = mouseDelta * sensitivity;
        frameVelocity = Vector2.Lerp(frameVelocity, rawFrameVelocity, Time.deltaTime * smoothing);

        velocity.x += frameVelocity.x;
        velocity.y = Mathf.Clamp(velocity.y + frameVelocity.y, lowerClamp, upperClamp);
    }

    void FixedUpdate()
    {
        // If any UI is open, prevent rotation
        if ((TutorialPopUp != null && TutorialPopUp.activeSelf) ||
            (WinScreen != null && WinScreen.activeSelf) ||
            (LoseScreen != null && LoseScreen.activeSelf))
            return;

        // Apply rotations after all updates
        transform.localRotation = Quaternion.Euler(-velocity.y, 0f, 0f);
        character.localRotation = Quaternion.Euler(0f, velocity.x, 0f);
    }
}
