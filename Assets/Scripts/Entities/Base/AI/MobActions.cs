using UnityEngine;
using UnityEngine.AI;
[RequireComponent(typeof(NavMeshAgent))]
public class MobActions : AIEntityActions
{
    //mob refers to a mobile AI
    [SerializeField]
    protected NavMeshAgent agent;
    protected void Awake()
    {
        agent = GetComponent<NavMeshAgent>();
    }
    public void Stop()
    {
        agent.isStopped = true;
    }
    public void MoveToLocation(Vector3 vector3, bool affectrotation = true)
    {
        agent.updateRotation = affectrotation;
        agent.isStopped = false;
        agent.SetDestination(vector3);
    }
}
