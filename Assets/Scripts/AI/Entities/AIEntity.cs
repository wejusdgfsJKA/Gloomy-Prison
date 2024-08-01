using UnityEngine.AI;

public class AIEntity : EntityBase
{
    protected NavMeshAgent agent;
    protected virtual void Awake()
    {
        agent = GetComponent<NavMeshAgent>();
        agent.speed = entityData.movespeed;
    }
}
