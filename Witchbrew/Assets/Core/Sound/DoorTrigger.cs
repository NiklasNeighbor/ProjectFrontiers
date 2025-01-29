using UnityEngine;

public class DoorTrigger : MonoBehaviour
{
    private void Start()
    {
        Debug.Log("DoorTrigger script is active.");
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            Debug.Log("Player entered the doorway collider!");

            // Call Crossfade directly
            if (AudioManager.Instance != null)
            {
                if (AudioManager.Instance.IsPlaying("tempInside"))
                {
                    AudioManager.Instance.Crossfade("tempOutside");
                }
                else if (AudioManager.Instance.IsPlaying("tempOutside"))
                {
                    AudioManager.Instance.Crossfade("tempInside");
                }
            }
            else
            {
                Debug.LogError("AudioManager instance is null!");
            }
        }
    }
}