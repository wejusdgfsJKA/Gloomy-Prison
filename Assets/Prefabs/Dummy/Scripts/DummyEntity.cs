using UnityEngine;
using UnityEngine.AI;
[RequireComponent(typeof(NavMeshAgent))]
public class DummyEntity : AIEntity
{
    [SerializeField]
    protected DummyAnimHandler animHandler;
    public void StopMoving()
    {
        agent.isStopped = true;
    }
    public void Swing(float angle)
    {
        animHandler.Swing(angle);
    }
    public void ChangeSwingAngle(float angle)
    {
        animHandler.ChangeSwingAngle(angle);
    }
    public void MoveToLocation(Vector3 location, bool affectrotation = true)
    {
        agent.updateRotation = affectrotation;
        agent.isStopped = false;
        agent.SetDestination(location);
    }
}
