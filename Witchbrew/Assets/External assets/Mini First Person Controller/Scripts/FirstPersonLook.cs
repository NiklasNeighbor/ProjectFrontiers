using UnityEngine;

public class FirstPersonLook : MonoBehaviour
{
    [SerializeField]
    Transform character;
    public GameObject TutorialPopUp; // Reference to the UI popup GameObject
    public float sensitivity = 2;
    public float smoothing = 1.5f;

    public float upperClamp = 90f; 
    public float lowerClamp = -90f;
    public bool uiActive;

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
        // If UI is active, disable camera movement
        if (uiActive)
        {
            Cursor.lockState = CursorLockMode.None;
            Cursor.visible = true;
            return;
        }
        else
        {
            Cursor.lockState = CursorLockMode.Locked;
            Cursor.visible = false;
        }

        // Get mouse input
        Vector2 mouseDelta = new Vector2(Input.GetAxisRaw("Mouse X"), Input.GetAxisRaw("Mouse Y"));

        // Smooth input
        Vector2 rawFrameVelocity = mouseDelta * sensitivity;
        frameVelocity = Vector2.Lerp(frameVelocity, rawFrameVelocity, Time.deltaTime * smoothing);

        // Apply to velocity and clamp vertical rotation
        velocity.x += frameVelocity.x;
        velocity.y = Mathf.Clamp(velocity.y + frameVelocity.y, lowerClamp, upperClamp);
    }
        void FixedUpdate()
    {
        // Apply rotation in FixedUpdate() to match physics updates
        transform.localRotation = Quaternion.Euler(-velocity.y, 0f, 0f);
        character.localRotation = Quaternion.Euler(0f, velocity.x, 0f);
    }
}
