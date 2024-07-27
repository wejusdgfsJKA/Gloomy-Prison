using UnityEngine;
[CreateAssetMenu(menuName = "ScriptableObjects/SoundData")]
[System.Serializable]
public class Sound : ScriptableObject
{
    public float intensity;
    public MutableData data;
}
[System.Serializable]
public struct MutableData
{
    public Transform source;
}
