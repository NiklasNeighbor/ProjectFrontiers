using UnityEngine;

public class PickUpScript : MonoBehaviour
{
    public Transform playerCam; // Reference to the player's camera
    public float maxDistance = 10f; // Maximum distance to pick up an object
    public float holdingDistance = 0.1f;
    public LayerMask whatIsPickUp; // Layer mask for pickable objects
    private SpringJoint joint;
    private GameObject pickedUpObject;
    private bool isPickingUp;

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.E))
        {
            PickUpItem();
        }
        else if (Input.GetKeyUp(KeyCode.E))
        {
            DropItem();
        }

        if (isPickingUp && joint != null)
        {
            // Update the connected anchor to follow the player's camera
            joint.connectedAnchor = playerCam.position + playerCam.forward * 2f;
        }

        // Visualize the raycast for debugging
        Debug.DrawRay(playerCam.position, playerCam.forward * maxDistance, Color.red, 1f);
    }

    void PickUpItem()
    {
        if (isPickingUp) return;

        RaycastHit hit;
        if (Physics.Raycast(playerCam.position, playerCam.forward, out hit, maxDistance, whatIsPickUp))
        {
            Debug.Log($"Hit object: {hit.collider.name}");
            pickedUpObject = hit.collider.gameObject;

            joint = pickedUpObject.AddComponent<SpringJoint>();
            joint.autoConfigureConnectedAnchor = false;
            joint.connectedAnchor = playerCam.position + playerCam.forward * 2f;

            float distanceFromPoint = Vector3.Distance(playerCam.position, pickedUpObject.transform.position);

            // Clamping distance for better control
            joint.maxDistance = Mathf.Clamp(distanceFromPoint * 0.8f, 0.1f, holdingDistance);
            joint.minDistance = Mathf.Clamp(distanceFromPoint * 0.25f, 0.1f, holdingDistance);

            // SpringJoint values
            joint.spring = 45f;
            joint.damper = 7f;
            joint.massScale = 45f;

            isPickingUp = true;
        }
        else
        {
            Debug.Log("No object detected.");
        }
    }

    void DropItem()
    {
        if (!isPickingUp || joint == null) return;

        Destroy(joint);
        pickedUpObject = null;
        isPickingUp = false;
    }
}
