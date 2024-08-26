using UnityEngine;
[CreateAssetMenu(menuName = "ScriptableObjects/SoundData")]
[System.Serializable]
public class Sound : ScriptableObject
{
    public float Intensity { get; protected set; }
    public SoundData Data { get; protected set; }
}
[System.Serializable]
public class SoundData
{
    public Transform Source { get; protected set; }
}
