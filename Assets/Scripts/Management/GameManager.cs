using UnityEngine;

public class GameManager : MonoBehaviour
{
    //overarching manager
    [SerializeField]
    protected AIEntity[] roster;
    [SerializeField]
    protected EntityData[] rosterData;
    [SerializeField]
    protected int[] InitialCreation;
    [SerializeField]
    EntityManager entityManager;
    //the integer determines how many of
    //each prefab should be instantiated into the level at the start
    [SerializeField]
    protected string[] teams;
    [SerializeField]
    DetectionManager detectionManager;
    private void OnEnable()
    {
        HandleEntityManager();
        HandleDetectionManager();
    }
    void HandleEntityManager()
    {
        entityManager.enabled = true;
        for (int i = 0; i < roster.Length; i++)
        {
            EntityManager.instance.AddToRoster(roster[i], rosterData[i].Name);
            for (int j = 0; j < InitialCreation[i]; j++)
            {
                EntityManager.instance.SpawnAndKill(roster[i]);
            }
        }
    }
    void HandleDetectionManager()
    {
        detectionManager.enabled = true;
        detectionManager.SetTeams(teams);
    }
}
