using UnityEngine;

[System.Serializable]
public class TargetData
{
    public bool Spotted { get; protected set; }
    public Transform Target { get; protected set; }
    public float TimeLastDetected { get; private set; }
    public Vector3 KnownPos { get; private set; }
    public EntityBase Entity { get; private set; }
    public TargetData(Transform target)
    {
        Target = target;
        Entity = EntityManager.Instance.Entities[target];
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
