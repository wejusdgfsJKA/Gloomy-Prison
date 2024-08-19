using UnityEngine;
[CreateAssetMenu(menuName = "ScriptableObjects/DetectionParams")]
public class DetectionParameters : ScriptableObject
{
    [Header("Sight")]
    public float SightRange = 30;
    public float SightAngle = 60;
    public LayerMask ObstructionMask = (1 << 0);
    [Header("Hearing")]
    public float HearingRange = 29;
    [Header("Proximity")]
    public float ProximityDetectionRange = 1.5f;
    [Header("Miscellaneous")]
    public float TimeToLose = 1;
    public float TimeToForget = 10;
    public float CheckInterval = 0.1f;
}
