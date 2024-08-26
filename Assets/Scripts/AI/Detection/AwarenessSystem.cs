using System.Collections;
using System.Collections.Generic;
using UnityEngine;
public class AwarenessSystem : MonoBehaviour
{
    [SerializeField]
    protected DetectionParameters parameters;
    protected WaitForSeconds wait;
    protected Coroutine coroutine;
    [SerializeField]
    protected Transform eye;
    public Dictionary<Transform, TargetData> Targets { get; protected set; } = new();
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
    protected IEnumerator enumerator()
    {
        while (true)
        {
            ProcessInformation();
            yield return wait;
            Detect();
        }
    }
    protected void Detect()
    {
        var teams = DetectionManager.Instance.Teams;
        if (teams != null)
        {
            for (int i = 0; i < teams.Length; i++)
            {
                if (teams[i] != transform.root.gameObject.layer.ToString())
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
    }
    protected void ProcessInformation()
    {
        Queue<Transform> queue = new Queue<Transform>();
        ClosestTarget = null;
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
            }
            else
            {
                float dist = (transform.position - ClosestTarget.KnownPos).sqrMagnitude;
                float newdist = (transform.position - data.Value.KnownPos).sqrMagnitude;
                if (newdist - dist > 1)
                {
                    ClosestTarget = data.Value;
                }
            }
        }
        while (queue.Count > 0)
        {
            //we can't remove targets in the foreach loop, we must do it separately
            Targets.Remove(queue.Dequeue());
        }
    }
    protected void HasDetected(Transform t)
    {
        //we have detected this target
        try
        {
            Targets[t].Refresh();
        }
        catch (KeyNotFoundException)
        {
            Targets.Add(t, new TargetData(t));
        }
    }
    public void Hear(Sound sound)
    {
        if (CanHear(sound))
        {
            HasDetected(sound.Data.Source);
        }
    }
    protected bool CanHear(Sound sound)
    {
        //can we hear a given sound? WIP
        return Vector3.Distance(transform.position, sound.Data.Source.position) <= parameters.HearingRange;
    }
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
    protected bool InProximity(Transform target)
    {
        //simple distance check for now
        return Vector3.Distance(transform.position, target.position) <= parameters.ProximityDetectionRange;
    }
}