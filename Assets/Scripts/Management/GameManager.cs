using UnityEngine;

public class GameManager : MonoBehaviour
{
    //overarching manager
    [SerializeField]
    protected string[] teams;//all the teams in the game
    [SerializeField]
    protected EntityData[] roster;
    [SerializeField]
    protected int[] InitialCreation;
    //the integer determines how many of
    //each prefab should be instantiated into the level at the start
    [SerializeField]
    EntityManager entityManager;
    [SerializeField]
    DetectionManager detectionManager;
    private void OnEnable()
    {
        HandleDetectionManager();
        HandleEntityManager();
    }
    void HandleDetectionManager()
    {
        detectionManager.enabled = true;
        detectionManager.SetTeams(teams);
    }
    void HandleEntityManager()
    {
        entityManager.enabled = true;
        for (int i = 0; i < roster.Length; i++)
        {
            EntityManager.instance.AddToRoster(roster[i]);
            for (int j = 0; j < InitialCreation[i]; j++)
            {
                EntityManager.instance.SpawnAndKill(roster[i].prefab);
            }
        }
    }
}
