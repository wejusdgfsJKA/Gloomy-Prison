using UnityEngine;

[System.Serializable]
public class TargetData
{
    public bool Spotted { get; protected set; }
    public Transform Target { get; protected set; }
    public float TimeLastDetected { get; protected set; }
    public Vector3 KnownPos { get; protected set; }
    public EntityBase Entity { get; protected set; }
    public TargetData(Transform _target)
    {
        Target = _target;
        Entity = EntityManager.Instance.Entities[_target];
        Refresh();
    }
    public void Refresh()
    {
        TimeLastDetected = Time.time;
        KnownPos = Target.position;
        Spotted = true;
    }
    public void WeakRefresh()
    {
        Spotted = false;
        KnownPos = Target.position;
    }
}
