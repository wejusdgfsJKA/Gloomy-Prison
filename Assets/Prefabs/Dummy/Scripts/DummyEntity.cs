using UnityEngine;
using UnityEngine.AI;
[RequireComponent(typeof(NavMeshAgent))]
public class DummyEntity : AIEntity
{
    protected NavMeshAgent agent;
    [SerializeField]
    protected DummyAnimHandler animHandler;
    protected void Awake()
    {
        agent = GetComponent<NavMeshAgent>();
        agent.speed = entityData.movespeed;
    }
    public void StopMoving()
    {
        agent.isStopped = true;
    }
    public void Swing(float angle)
    {
        animHandler.Swing(angle);
    }
    public void MoveToLocation(Vector3 location)
    {
        agent.isStopped = false;
        agent.SetDestination(location);
    }
}
