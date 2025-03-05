using UnityEngine;

[System.Serializable]
public class TargetData
{
    /// <summary>
    /// Have we spotted this target?
    /// </summary>
    public bool Spotted { get; protected set; }
    /// <summary>
    /// The actual target.
    /// </summary>
    public Transform Target { get; protected set; }
    /// <summary>
    /// When we last detected this entity.
    /// </summary>
    public float TimeLastDetected { get; protected set; }
    /// <summary>
    /// Where we think the entity is.
    /// </summary>
    public Vector3 KnownPos { get; protected set; }
    /// <summary>
    /// The reference to the entity.
    /// </summary>
    public EntityBase Entity { get; protected set; }
    public TargetData(Transform target)
    {
        Target = target;
        Entity = EntityManager.Instance.Entities[target];
        Refresh();
    }
    /// <summary>
    /// Fires when the entity is detected.
    /// </summary>
    public void Refresh()
    {
        TimeLastDetected = Time.time;
        KnownPos = Target.position;
        Spotted = true;
    }
    /// <summary>
    /// Fires when the entity has just left detection.
    /// </summary>
    public void WeakRefresh()
    {
        Spotted = false;
        KnownPos = Target.position;
    }
}
