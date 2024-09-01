using UnityEngine;

public class GameManager : MonoBehaviour
{
    //overarching manager
    [SerializeField]
    protected string[] teams;//all the teams in the game
    [SerializeField]
    protected EntityData[] roster;
    [SerializeField]
    protected int[] initialCreation;
    //the integer determines how many of
    //each prefab should be instantiated into the level at the start
    [SerializeField]
    protected EntityManager entityManager;
    [SerializeField]
    protected DetectionManager detectionManager;
    protected void OnEnable()
    {
        HandleDetectionManager();
        HandleEntityManager();
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
            for (int j = 0; j < initialCreation[i]; j++)
            {
                EntityManager.Instance.SpawnAndKill(roster[i].Prefab);
            }
        }
    }
}
