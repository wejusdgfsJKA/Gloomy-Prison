using UnityEngine;

[RequireComponent(typeof(InteractableManager))]
[RequireComponent(typeof(EntityManager))]
[RequireComponent(typeof(DetectionManager))]
public class GameManager : MonoBehaviour
{
    //overarching manager
    [SerializeField]
    protected string[] teams;//all the teams in the game
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
        HandleInteractableManager();
        PlayerSettings.interactionDistance = 10;
        PlayerSettings.xSens = 1;
        PlayerSettings.ySens = 1;
    }
    protected void HandleDetectionManager()
    {
        detectionManager.enabled = true;
        detectionManager.SetTeams(teams);
    }
    protected void HandleEntityManager()
    {
        entityManager.enabled = true;
        for (int i = 0; i < roster.Length; i++)
        {
            EntityManager.Instance.AddToRoster(roster[i]);
        }
    }
    protected void HandleInteractableManager()
    {
        interactableManager.Awake();
    }
}
