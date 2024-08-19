using UnityEngine;
using UnityEngine.AI;
public class AIEntity : EntityBase
{
    protected NavMeshAgent agent;
    protected override void Awake()
    {
        base.Awake();
        agent = GetComponent<NavMeshAgent>();
    }
    public void MoveToLocation(Vector3 location, bool affectrotation = true)
    {
        agent.updateRotation = affectrotation;
        agent.isStopped = false;
        agent.speed = entityData.movespeed;
        agent.SetDestination(location);
    }
    public void StopMoving()
    {
        agent.isStopped = true;
    }
    public void FaceTarget(Vector3 target)
    {
        transform.LookAt(new Vector3(target.x, transform.position.y, target.z));
    }
}
