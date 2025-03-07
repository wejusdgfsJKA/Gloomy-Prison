using UnityEngine;
[CreateAssetMenu(menuName = "ScriptableObjects/SoundData")]
[System.Serializable]
public class Sound : ScriptableObject
{
    /// <summary>
    /// Currently unused.
    /// </summary>
    public float Intensity { get; protected set; }
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
