using UnityEngine;

[System.Serializable]
public class TargetData
{
    public bool spotted { get; protected set; }
    public Transform target { get; protected set; }
    public float TimeLastDetected { get; private set; }
    public Vector3 KnownPos { get; private set; }
    public EntityBase data { get; private set; }
    public TargetData(Transform target)
    {
        this.target = target;
        data = EntityManager.instance.entities[target];
        Refresh();
    }
    public void Refresh()
    {
        TimeLastDetected = Time.time;
        KnownPos = target.position;
        spotted = true;
    }
    public void WeakRefresh()
    {
        spotted = false;
        KnownPos = target.position;
    }
}
