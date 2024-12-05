using UnityEngine;
using UnityEngine.AI;
[RequireComponent(typeof(NavMeshAgent))]
[RequireComponent(typeof(EntityBase))]
public class MobActions : AIEntityActions
{
    //mob refers to a mobile AI
    //we are not putting anything attack related here for now,
    //because we might have something passive like the sack rats
    protected NavMeshAgent agent;
    protected EntityBase entity;
    public Vector3 Destination
    {
        get
        {
            return agent.destination;
        }
    }
    protected void Awake()
    {
        entity = GetComponent<EntityBase>();
        agent = GetComponent<NavMeshAgent>();
    }
    public void Stop()
    {
        agent.isStopped = true;
    }
    public void MoveToLocation(Vector3 _vector3, bool _affectrotation = true)
    {
        agent.updateRotation = _affectrotation;
        agent.isStopped = false;
        agent.SetDestination(_vector3);
    }
}
