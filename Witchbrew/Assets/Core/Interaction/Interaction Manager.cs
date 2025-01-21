using Unity.VisualScripting;
using UnityEngine;

public class InteractionManager : MonoBehaviour
{
    public Camera playerCamera;
    public float pickupDistance = 3f;
    public float interactionRange = 5f;
    public float jointSpring = 500f;
    public float jointDamper = 50f;

    private GameObject pickedObject = null;
    private Rigidbody pickedRigidbody = null;
    private ConfigurableJoint configurableJoint = null;
    private RaycastHit hit;
    private Vector3 previousPosition;
    private Vector3 previousVelocity;  // Store the previous velocity before dropping
    private bool isHolding = false;

    void Update()
    {
        Ray ray = playerCamera.ScreenPointToRay(Input.mousePosition);
        
        if (Input.GetKeyDown(KeyCode.E))
        {
            if (Physics.Raycast(ray, out hit, interactionRange) && !isHolding)
            {
                TryPickupObject();
            }
            else if (isHolding)
            {
                DropObject();
            }
        }

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
        } else if (isHolding && pickedObject == null)
        {
            isHolding = false;
        }
    }


    void TryPickupObject()
    {
        if (hit.rigidbody != null && hit.collider.CompareTag("Pickup"))
        {
            pickedObject = hit.collider.gameObject;
            pickedRigidbody = pickedObject.GetComponent<Rigidbody>();

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
    }

    void UpdateJointTargetPosition()
    {
        if (configurableJoint != null && pickedObject != null)
        {
            Vector3 targetPosition = CalculateTargetAnchor();

            // Smoothly move the object to the target position
            pickedRigidbody.position.Set(targetPosition.x, targetPosition.y, targetPosition.z);

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
}


