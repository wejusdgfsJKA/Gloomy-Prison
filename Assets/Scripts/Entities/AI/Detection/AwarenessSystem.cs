using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
/// <summary>
/// Handles target awareness.
/// </summary>
public class AwarenessSystem : MonoBehaviour
{
    [SerializeField] protected DetectionParameters parameters;
    /// <summary>
    /// Where does the entity see from?
    /// </summary>
    [SerializeField] protected Transform eye;
    protected WaitForSeconds wait;
    protected Coroutine coroutine;
    /// <summary>
    /// All the targets we have knowledge of. Includes targets
    /// we have spotted some time ago but we have no recent
    /// information on.
    /// </summary>
    public Dictionary<Transform, TargetData> Targets { get; protected set; } = new();
    /// <summary>
    /// The target closest to us.
    /// </summary>
    public TargetData ClosestTarget { get; protected set; }
    protected void Awake()
    {
        wait = new WaitForSeconds(parameters.CheckInterval);
    }
    protected void OnEnable()
    {
        DetectionManager.Instance.RegisterListener(this);
        coroutine = StartCoroutine(enumerator());
    }
    protected void OnDisable()
    {
        DetectionManager.Instance.DeRegisterListener(this);
        if (coroutine != null)
        {
            StopCoroutine(coroutine);
        }
        Targets.Clear();
    }
    /// <summary>
    /// Update loop.
    /// </summary>
    /// <returns></returns>
    protected IEnumerator enumerator()
    {
        while (true)
        {
            ProcessInformation();
            yield return wait;
            Detect();
        }
    }
    /// <summary>
    /// Handles visual detection.
    /// </summary>
    protected void Detect()
    {
        var teams = DetectionManager.Instance.Teams;
        for (int i = 0; i < teams.Count; i++)
        {
            if (teams[i] != transform.root.gameObject.layer)
            {
                //this is an enemy team
                var team = DetectionManager.Instance.Targets[teams[i]];
                foreach (Transform entity in team)
                {
                    //we go through all entities in this team and check if we
                    //can detect them by sight or proximity
                    if (InProximity(entity) || CanSee(entity))
                    {
                        //we can see this entity or it is very close to us
                        HasDetected(entity);
                    }
                    //hearing is handled separately
                }
            }
        }
    }
    /// <summary>
    /// Remove targets which have been destroyed or have not been detected for a 
    /// long time. Refresh data on targets which have been recently spotted and update
    /// closest target.
    /// </summary>
    protected void ProcessInformation()
    {
        Queue<Transform> queue = new Queue<Transform>();

        ClosestTarget = null;
        float distToClosest = -1;

        //we determine the closest target to us here for efficiency's sake
        foreach (var data in Targets)
        {
            if (!data.Key.gameObject.activeSelf)
            {
                //this entity has been deactivated
                queue.Enqueue(data.Key);
                continue;
            }

            if (Time.time - data.Value.TimeLastDetected >= parameters.TimeToForget)
            {
                //we will forget this target
                queue.Enqueue(data.Key);
                continue;
            }

            if (data.Value.Spotted || Time.time - data.Value.TimeLastDetected <= parameters.TimeToLose)
            {
                data.Value.WeakRefresh();
            }

            if (ClosestTarget == null || !Targets.ContainsKey(ClosestTarget.Target))
            {
                ClosestTarget = data.Value;
                distToClosest = (transform.position - ClosestTarget.KnownPos).
                    sqrMagnitude;
            }
            else
            {
                float newDist = (transform.position - data.Value.KnownPos).sqrMagnitude;
                if (newDist - distToClosest < 1)
                {
                    //we have a new closest target
                    ClosestTarget = data.Value;
                    distToClosest = (transform.position - ClosestTarget.KnownPos).
                        sqrMagnitude;
                }
            }
        }

        //remove targets which were destroyed or we forgot about
        while (queue.Count > 0)
        {
            //we can't remove targets in the foreach loop, we must do it separately
            Targets.Remove(queue.Dequeue());
        }
    }
    /// <summary>
    /// We have detected a target.
    /// </summary>
    /// <param name="target">The target we have detected.</param>
    protected void HasDetected(Transform target)
    {
        TargetData data;
        if (Targets.TryGetValue(target, out data))
        {
            data.Refresh();
        }
        else
        {
            Targets.Add(target, new TargetData(target));
        }
    }
    /// <summary>
    /// Notify this entity of a sound.
    /// </summary>
    /// <param name="sound">The sound we heard.</param>
    public void Hear(Sound sound)
    {
        if (CanHear(sound))
        {
            HasDetected(sound.Data.Source);
        }
    }
    /// <summary>
    /// Can we hear a given sound? WIP
    /// </summary>
    /// <param name="sound">The sound in question.</param>
    /// <returns>True if we can hear the sound.</returns>
    protected bool CanHear(Sound sound)
    {
        return Vector3.Distance(transform.position, sound.Data.Source.position) <= parameters.HearingRange;
    }
    /// <summary>
    /// Checks if we can see a target.
    /// </summary>
    /// <param name="target">The target in question.</param>
    /// <returns>True if we can see a target.</returns>
    protected bool CanSee(Transform target)
    {
        Vector3 VectorToTarget = target.position - transform.position;
        if (VectorToTarget.sqrMagnitude > parameters.SightRange * parameters.SightRange)
        {
            //the target is out of range
            return false;
        }
        VectorToTarget.Normalize();
        if (Vector3.Dot(VectorToTarget, eye.forward) < Mathf.Cos(Mathf.Deg2Rad * parameters.SightAngle))
        {
            //the target is not in our field of view
            return false;
        }
        if (Physics.Linecast(transform.position, target.position, parameters.ObstructionMask))
        {
            //the target is obstructed
            return false;
        }
        return true;
    }
    /// <summary>
    /// Check if an entity is very close to us.
    /// </summary>
    /// <param name="target">The entity in question.</param>
    /// <returns>True if the entity is in range of proximity detection.</returns>
    protected bool InProximity(Transform target)
    {
        //simple distance check for now
        return Vector3.Distance(transform.position, target.position) <= parameters.ProximityDetectionRange;
    }
}

#if UNITY_EDITOR
[CustomEditor(typeof(AwarenessSystem))]
public class AwarenessViewer : Editor
{
    private void OnSceneGUI()
    {
        AwarenessSystem t = (AwarenessSystem)target;
        if (t.ClosestTarget != null)
        {
            Handles.DrawLine(t.transform.position, t.ClosestTarget.KnownPos);
        }
    }
}
#endif