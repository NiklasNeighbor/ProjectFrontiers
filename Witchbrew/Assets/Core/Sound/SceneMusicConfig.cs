using UnityEngine;

[CreateAssetMenu(fileName = "SceneMusicConfig", menuName = "Audio/SceneMusicConfig")]
public class SceneMusicConfig : ScriptableObject
{
    public string sceneName; // Name of the scene
    public string songKey;   // Key of the song to play at the start of the scene
}
