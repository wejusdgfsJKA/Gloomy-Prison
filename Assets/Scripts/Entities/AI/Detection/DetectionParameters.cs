using UnityEngine;
[CreateAssetMenu(menuName = "ScriptableObjects/DetectionParams")]
public class DetectionParameters : ScriptableObject
{
    [field: Header("Sight")]
    [field: SerializeField]
    public float SightRange { get; protected set; } = 30;
    [field: SerializeField]
    public float SightAngle { get; protected set; } = 60;
    [field: SerializeField]
    public LayerMask ObstructionMask { get; protected set; } = (1 << 0);
    [field: Header("Hearing")]
    [field: SerializeField]
    public float HearingRange { get; protected set; } = 29;
    [field: Header("Proximity")]
    [field: SerializeField]
    public float ProximityDetectionRange { get; protected set; } = 1.5f;
    [field: Header("Miscellaneous")]
    [field: SerializeField]
    public float TimeToLose { get; protected set; } = 1;
    public float TimeToForget { get; protected set; } = 10;
    [field: SerializeField]
    public float CheckInterval { get; protected set; } = 0.1f;
}
