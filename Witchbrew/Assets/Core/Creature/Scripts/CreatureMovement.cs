using UnityEngine;

public class CreatureMovement : MonoBehaviour
{
    [Header("Movement Settings")]
    public float moveSpeed = 2f; // Speed of movement
    public float rotationSpeed = 5f; // Speed of turning
    public float wanderDelay = 2f; // Time to wait before changing direction

    [Header("Boundary Settings")]
    public Vector3 centerPoint; // Center point of the movement area
    public Vector3 movementBounds = new Vector3(10f, 0f, 10f); // movement area

    private Vector3 targetPosition; // The next position to move toward
    private float timeToNextMove; // Timer for changing direction

    void Start()
    {
        PickRandomTargetPosition();
    }

    void Update()
    {
        // Check if its time to pick a new direction
        if (timeToNextMove <= 0f)
        {
            PickRandomTargetPosition();
            timeToNextMove = wanderDelay;
        }
        else
        {
            timeToNextMove -= Time.deltaTime;
        }

        // Move toward the target position
        MoveTowardsTarget();
    }

    void PickRandomTargetPosition()
    {
        float randomX = Random.Range(-movementBounds.x / 2, movementBounds.x / 2);
        float randomZ = Random.Range(-movementBounds.z / 2, movementBounds.z / 2);
        float newY = transform.position.y; // Keep the same height

        // Calculate the new position within bounds
        targetPosition = new Vector3(centerPoint.x + randomX, newY, centerPoint.z + randomZ);
    }

    void MoveTowardsTarget()
    {
        Vector3 direction = (targetPosition - transform.position).normalized;

        // Rotate towards the target
        if (direction != Vector3.zero)
        {
            Quaternion targetRotation = Quaternion.LookRotation(direction);
            transform.rotation = Quaternion.Slerp(transform.rotation, targetRotation, rotationSpeed * Time.deltaTime);
        }

        // Move forward
        transform.position += transform.forward * moveSpeed * Time.deltaTime;

        // Check if the creature is close enough to the target position
        if (Vector3.Distance(transform.position, targetPosition) < 0.5f)
        {
            timeToNextMove = 0f; // Immediately pick a new target
        }
    }

    void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.green;
        Gizmos.DrawWireCube(centerPoint, new Vector3(movementBounds.x, 0.1f, movementBounds.z));
    }
}
