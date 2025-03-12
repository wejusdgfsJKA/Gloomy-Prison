using UnityEngine;
[CreateAssetMenu(menuName = "ScriptableObjects/SoundData")]
[System.Serializable]
public class Sound : ScriptableObject
{
    /// <summary>
    /// Add the intensity to the hearing range of the listener.
    /// </summary>
    public float Intensity { get; protected set; } = 0;
    public SoundData Data { get; protected set; }
}
[System.Serializable]
public class SoundData
{
    /// <summary>
    /// The object who made the sound.
    /// </summary>
    public Transform Source { get; protected set; }
}
