using UnityEngine;

public class FirstPersonLook : MonoBehaviour
{
    [SerializeField]
    Transform character;
    public RecipeBookManager RecipeBookManager;
    public GameObject TutorialPopUp; // Reference to the UI popup GameObject
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

        // If the UI popup is active, freeze the camera and unlock the cursor
        if (TutorialPopUp != null && TutorialPopUp.activeSelf)
        {
            Cursor.lockState = CursorLockMode.None;
            Cursor.visible = true;
            return; // Exit update to prevent camera movement
        }
        else
        {
            Cursor.lockState = CursorLockMode.Locked;
            Cursor.visible = false;
        }

        if (!RecipeBookManager.isRecipeBookOpen)
        {
            Vector2 mouseDelta = new Vector2(Input.GetAxisRaw("Mouse X"), Input.GetAxisRaw("Mouse Y"));
            Vector2 rawFrameVelocity = Vector2.Scale(mouseDelta, Vector2.one * sensitivity);
            frameVelocity = Vector2.Lerp(frameVelocity, rawFrameVelocity, 1 / smoothing);

            // Update the camera rotation
            velocity += frameVelocity;
            velocity.y = Mathf.Clamp(velocity.y, lowerClamp, upperClamp);
        }
    }

    void LateUpdate()
    {
        if (TutorialPopUp != null && TutorialPopUp.activeSelf) return;

        // Apply rotations after all updates
        transform.localRotation = Quaternion.AngleAxis(-velocity.y, Vector3.right);
        character.localRotation = Quaternion.AngleAxis(velocity.x, Vector3.up);
    }
}
