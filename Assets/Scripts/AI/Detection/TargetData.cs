using UnityEngine;

[System.Serializable]
public class TargetData
{
    public bool spotted { get; protected set; }
    public Transform target { get; private set; }
    public float TimeLastDetected { get; private set; }
    public Vector3 KnownPos { get; private set; }
    public TargetData(Transform target)
    {
        this.target = target;
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
