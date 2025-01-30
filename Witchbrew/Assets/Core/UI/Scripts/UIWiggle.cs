using UnityEngine;

public class UIWiggle : MonoBehaviour
{
    [Header("Position Wiggle Settings")]
    public float positionIntensity = 10f; // How much the UI element wiggles in position
    public float positionSpeed = 2f;      // How fast the UI element wiggles in position

    [Header("Rotation Wiggle Settings")]
    public float rotationIntensity = 5f; // How much the UI element wiggles in rotation
    public float rotationSpeed = 1.5f;   // How fast the UI element wiggles in rotation

    [Header("Randomness Settings")]
    public float positionRandomness = 0.5f; // Randomness for position wiggle
    public float rotationRandomness = 0.5f; // Randomness for rotation wiggle

    private RectTransform rectTransform;
    private Vector2 originalPosition;
    private Quaternion originalRotation;

    // Random offsets for position and rotation
    private float positionOffset;
    private float rotationOffset;

    void Start()
    {
        // Get the RectTransform component
        rectTransform = GetComponent<RectTransform>();

        // Store the original position and rotation
        originalPosition = rectTransform.anchoredPosition;
        originalRotation = rectTransform.localRotation;

        // Initialize random offsets
        positionOffset = Random.Range(0f, 100f);
        rotationOffset = Random.Range(0f, 100f);
    }

    void FixedUpdate()
    {
        WiggleUI();
    }

    void WiggleUI()
    {
        float time = Time.time;

        // Wiggle position
        WigglePosition(time);

        // Wiggle rotation
        WiggleRotation(time);
    }

    void WigglePosition(float time)
    {
        // Calculate a wiggle offset using sine and cosine functions with randomness
        Vector2 offset = new Vector2(
            Mathf.Sin(time * positionSpeed + positionOffset) * positionIntensity * (1 + Random.Range(-positionRandomness, positionRandomness)),
            Mathf.Cos(time * positionSpeed + positionOffset) * positionIntensity * (1 + Random.Range(-positionRandomness, positionRandomness))
        );

        // Apply the offset to the original position
        rectTransform.anchoredPosition = originalPosition + offset;
    }

    void WiggleRotation(float time)
    {
        // Calculate a wiggle rotation using sine function with randomness
        float rotation = Mathf.Sin(time * rotationSpeed + rotationOffset) * rotationIntensity * (1 + Random.Range(-rotationRandomness, rotationRandomness));

        // Apply the rotation to the original rotation
        rectTransform.localRotation = originalRotation * Quaternion.Euler(0, 0, rotation);
    }
}