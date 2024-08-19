using UnityEngine;

public class Settings : MonoBehaviour
{
    public float TimedBlockWindow = 1;
    public float TimedBlockStaminaCostCoefficient;
    public static Settings instance;
    private void Awake()
    {
        if (instance == null)
        {
            instance = this;
        }
    }
}
