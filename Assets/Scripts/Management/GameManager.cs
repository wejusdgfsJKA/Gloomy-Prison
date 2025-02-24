using UnityEngine;

/// <summary>
/// Overarching manager.
/// </summary>
[RequireComponent(typeof(InteractableManager))]
[RequireComponent(typeof(EntityManager))]
[RequireComponent(typeof(DetectionManager))]
public class GameManager : MonoBehaviour
{
    [SerializeField]
    protected EntityData[] roster;
    protected EntityManager entityManager;
    protected DetectionManager detectionManager;
    protected InteractableManager interactableManager;
    protected void Awake()
    {
        entityManager = GetComponent<EntityManager>();
        detectionManager = GetComponent<DetectionManager>();
        interactableManager = GetComponent<InteractableManager>();
    }
    protected void OnEnable()
    {
        HandleDetectionManager();
        HandleEntityManager();
        PlayerSettings.interactionDistance = 10;
        PlayerSettings.xSens = 1;
        PlayerSettings.ySens = 1;
    }
    /// <summary>
    /// Initialize the detection manager.
    /// </summary>
    protected void HandleDetectionManager()
    {
        detectionManager.enabled = true;
    }
    /// <summary>
    /// Initialize the entity manager.
    /// </summary>
    protected void HandleEntityManager()
    {
        entityManager.enabled = true;
        for (int i = 0; i < roster.Length; i++)
        {
            entityManager.AddToRoster(roster[i]);
        }
    }
}
