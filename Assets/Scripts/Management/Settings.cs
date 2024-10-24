using UnityEngine;

public class Settings : MonoBehaviour
{
    public static Settings Instance { get; protected set; }
    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
        }
    }
}
