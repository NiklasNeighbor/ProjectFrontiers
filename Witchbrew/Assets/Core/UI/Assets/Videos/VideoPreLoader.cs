using UnityEngine;
using UnityEngine.Video;

public class VideoPreloader : MonoBehaviour
{
    public VideoPlayer videoPlayer; // Reference to the VideoPlayer component

    private void Start()
    {
        if (videoPlayer == null)
        {
            videoPlayer = GetComponent<VideoPlayer>();
        }

        if (videoPlayer != null)
        {
            // Subscribe to the prepareCompleted event
            videoPlayer.prepareCompleted += OnVideoPrepared;

            // Start preparing the video
            videoPlayer.Prepare();
        }
        else
        {
            Debug.LogError("VideoPlayer component not found.");
        }
    }

    private void OnVideoPrepared(VideoPlayer vp)
    {
        Debug.Log("Video is fully prepared and ready to play.");

        // Unsubscribe from the event to avoid multiple calls
        videoPlayer.prepareCompleted -= OnVideoPrepared;

        // Play the video (optional)
        videoPlayer.Play();
    }
}
