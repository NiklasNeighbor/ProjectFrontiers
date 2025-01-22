using Unity.Mathematics;
using Unity.VisualScripting;
using UnityEngine;

public class InteractionManager : MonoBehaviour
{
    public StashHandler stashHandler;
    public Camera playerCamera;
    public float pickupDistance = 3f;
    public float interactionRange = 5f;
    public float jointSpring = 500f;
    public float jointDamper = 50f;
    public GameObject pickedObject = null;
    public Rigidbody pickedRigidbody = null;
    public RaycastHit hit;
    public bool isHolding = false;

    private ConfigurableJoint configurableJoint = null;
    private Vector3 previousPosition;
    private Vector3 previousVelocity;  // Store the previous velocity before dropping
    private GameObject currentStash;

    void Update()
    {
        Ray ray = playerCamera.ScreenPointToRay(Input.mousePosition);
        
        if (Input.GetKeyDown(KeyCode.E))
        {
            if (Physics.Raycast(ray, out hit, interactionRange) && !isHolding)
            {
                TryInteract();
            }
            else if (isHolding)
            {
                DropObject();
            }
        }

        Debug.Log(isHolding);

    }

    void FixedUpdate()
    {
        if (isHolding && pickedObject != null)
        {
            UpdateJointTargetPosition();
            CalculateMomentum();
        }
    }

    void LateUpdate()
    {
        if (isHolding && pickedObject != null)
        {
            // Recalculate target anchor based on the latest camera position
            configurableJoint.connectedAnchor = CalculateTargetAnchor();
        }

        // Ensure isHolding remains consistent
        if (isHolding && (pickedObject == null || !pickedObject.activeInHierarchy))
        {
            ResetHoldingState();
        }
    }


    void TryInteract()
    {
        if (hit.rigidbody != null && hit.collider.CompareTag("Pickup"))
        {
            pickedObject = hit.collider.gameObject;
            pickedRigidbody = pickedObject.GetComponent<Rigidbody>();
            Holding();

           
        }  else if (hit.collider != null && hit.collider.CompareTag("Stash"))
        {
            currentStash = hit.collider.gameObject;
            stashHandler = currentStash.GetComponent<StashHandler>();
            stashHandler.SpawnObject();
        }
    }


    public void Holding()
    {
        // Add ConfigurableJoint
        configurableJoint = pickedObject.AddComponent<ConfigurableJoint>();
        configurableJoint.autoConfigureConnectedAnchor = false;
        configurableJoint.anchor = Vector3.zero;
        configurableJoint.connectedAnchor = playerCamera.transform.position + playerCamera.transform.forward * pickupDistance;

        // Set motion constraints for the joint
        configurableJoint.xMotion = ConfigurableJointMotion.Limited;
        configurableJoint.yMotion = ConfigurableJointMotion.Limited;
        configurableJoint.zMotion = ConfigurableJointMotion.Limited;

        // Allow free rotation
        configurableJoint.angularXMotion = ConfigurableJointMotion.Free;
        configurableJoint.angularYMotion = ConfigurableJointMotion.Free;
        configurableJoint.angularZMotion = ConfigurableJointMotion.Free;

        // Configure joint's drive for position control
        JointDrive jointDrive = new JointDrive
        {
            positionSpring = jointSpring,
            positionDamper = jointDamper,
            maximumForce = Mathf.Infinity
        };
        configurableJoint.xDrive = jointDrive;
        configurableJoint.yDrive = jointDrive;
        configurableJoint.zDrive = jointDrive;

        // Disable gravity while holding
        pickedRigidbody.useGravity = false;

        // Store the initial velocity and position for momentum calculation
        previousPosition = pickedObject.transform.position;
        previousVelocity = pickedRigidbody.velocity;

        isHolding = true;        
    }

    void UpdateJointTargetPosition()
    {
        if (configurableJoint != null && pickedObject != null)
        {
            Vector3 targetPosition = CalculateTargetAnchor();

            // Smoothly move the object to the target position
            pickedRigidbody.MovePosition(Vector3.Lerp(
            pickedObject.transform.position,
            targetPosition,
            Time.fixedDeltaTime * .1f
            ));

            // Update joint anchor for physics consistency
            configurableJoint.connectedAnchor = targetPosition;
        }
    }
    Vector3 CalculateTargetAnchor()
    {
        // Calculate the desired anchor position relative to the camera
        return playerCamera.transform.position + playerCamera.transform.forward * pickupDistance;
    }

    void DropObject()
    {
        if (pickedObject != null && configurableJoint != null)
        {
            // Remove the ConfigurableJoint component
            Destroy(configurableJoint);
            configurableJoint = null;

            // Re-enable gravity
            pickedRigidbody.useGravity = true;

            // Apply the last calculated momentum to the object's Rigidbody
            pickedRigidbody.velocity = previousVelocity;

            // Clear references and reset state
            pickedObject = null;
            pickedRigidbody = null;
            isHolding = false;
        }
    }

    void CalculateMomentum()
    {
        // Update previous velocity based on the object's position and velocity
        Vector3 currentPosition = playerCamera.transform.position;
        Vector3 currentVelocity = (currentPosition - previousPosition) / Time.deltaTime;

        previousVelocity = currentVelocity;
        previousPosition = currentPosition;
    }

    void ResetHoldingState()
    {
        isHolding = false;
        pickedObject = null;
        pickedRigidbody = null;
        if (configurableJoint != null)
        {
            Destroy(configurableJoint);
            configurableJoint = null;
        }
        Debug.Log("Reset holding state due to object disappearance.");
    }
}


