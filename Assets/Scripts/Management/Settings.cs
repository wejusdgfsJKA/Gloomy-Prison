using UnityEngine;

public class Settings : MonoBehaviour
{
    [field: SerializeField]
    public float TimedBlockWindow { get; protected set; } = 1;
    [field: SerializeField]
    public float TimedBlockStaminaCostCoefficient { get; protected set; }
    public static Settings Instance { get; protected set; }
    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
        }
    }
}
