using UnityEngine;
using UnityEngine.AI;
[RequireComponent(typeof(NavMeshAgent))]
public class DummyEntity : AIEntity
{
    public float DodgeSpeed;
    public Transform WeaponPivot;
    public override void StopBlocking()
    {
        base.StopBlocking();
        animHandler.StopBlocking();
    }
    public void Swing()
    {
        StopBlocking();
        animHandler.Swing();
    }
    public void Push()
    {

    }
    public void Feint()
    {
        animHandler.Feint();
    }
    public void ChangeSwingAngle(float angle)
    {
        animHandler.ChangeSwingAngle(angle);
    }
    public override void BeginBlocking()
    {
        base.BeginBlocking();
        animHandler.StartBlocking();
    }
    public void Dodge(Vector3 direction)
    {
        agent.speed = DodgeSpeed;
        agent.isStopped = false;
        agent.updateRotation = false;
        agent.SetDestination(transform.position + DodgeSpeed * direction);
    }
    public void AimWeapon(Vector3 vector3)
    {
        WeaponPivot?.LookAt(vector3);
    }
}
