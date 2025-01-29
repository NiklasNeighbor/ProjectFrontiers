using System.Collections.Generic;
using UnityEngine;

public class FirstPersonMovement : MonoBehaviour
{
    public float speed = 5f;

    [Header("Running")]
    public bool canRun = true;
    public bool IsRunning { get; private set; }
    public float runSpeed = 9f;
    public KeyCode runningKey = KeyCode.LeftShift;

    private Vector2 inputDirection;
    private Rigidbody rigidbody;

    public List<System.Func<float>> speedOverrides = new List<System.Func<float>>();

    void Awake()
    {
        rigidbody = GetComponent<Rigidbody>();
        rigidbody.interpolation = RigidbodyInterpolation.None; // Enable interpolation
    }

    void Update()
    {
        // Read input in Update() for responsiveness
        inputDirection = Vector2.ClampMagnitude(new Vector2(Input.GetAxis("Horizontal"), Input.GetAxis("Vertical")), 1);

        // Determine running state
        IsRunning = canRun && Input.GetKey(runningKey);
    }

    void FixedUpdate()
    {
        // Get target speed
        float targetSpeed = speedOverrides.Count > 0
            ? speedOverrides[speedOverrides.Count - 1]()
            : (IsRunning ? runSpeed : speed);

        // Calculate target velocity
        Vector3 targetVelocity = transform.rotation * new Vector3(inputDirection.x, 0, inputDirection.y) * targetSpeed;

        // Move the Rigidbody smoothly
        Vector3 newPosition = rigidbody.position + targetVelocity * Time.fixedDeltaTime;
        rigidbody.MovePosition(newPosition);
    }
}
