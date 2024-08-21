using UnityEngine;
using UnityEngine.AI;
[RequireComponent(typeof(NavMeshAgent))]
public class MobActions : AIEntityActions
{
    //mob refers to a mobile AI
    //we are not putting anything attack related here for now,
    //because we might have something passive like the sack rats
    [SerializeField]
    protected NavMeshAgent agent;
    protected void Awake()
    {
        agent = GetComponent<NavMeshAgent>();
    }
    protected void Stop()
    {
        agent.isStopped = true;
    }
    protected void MoveToLocation(Vector3 vector3, bool affectrotation = true)
    {
        agent.updateRotation = affectrotation;
        agent.isStopped = false;
        agent.SetDestination(vector3);
    }
}
