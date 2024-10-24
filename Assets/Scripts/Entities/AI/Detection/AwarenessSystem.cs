using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
public class AwarenessSystem : MonoBehaviour
{
    [SerializeField] protected DetectionParameters parameters;
    [SerializeField] protected Transform eye;
    protected WaitForSeconds wait;
    protected Coroutine coroutine;
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
        var _teams = DetectionManager.Instance.Teams;
        if (_teams != null)
        {
            for (int i = 0; i < _teams.Length; i++)
            {
                if (_teams[i] != transform.root.gameObject.layer.ToString())
                {
                    //this is an enemy team
                    var _team = DetectionManager.Instance.Targets[_teams[i]];
                    foreach (Transform _entity in _team)
                    {
                        //we go through all entities in this team and check if we
                        //can detect them by sight or proximity
                        if (InProximity(_entity) || CanSee(_entity))
                        {
                            //we can see this entity or it is very close to us
                            HasDetected(_entity);
                        }
                        //hearing is handled separately
                    }
                }
            }
        }
    }
    protected void ProcessInformation()
    {
        Queue<Transform> _queue = new Queue<Transform>();
        ClosestTarget = null;
        //we determine the closest target to us here for efficiency's sake
        foreach (var _data in Targets)
        {
            if (!_data.Key.gameObject.activeSelf)
            {
                //this entity has been deactivated
                _queue.Enqueue(_data.Key);
                continue;
            }
            if (Time.time - _data.Value.TimeLastDetected >= parameters.TimeToForget)
            {
                //we will forget this target
                _queue.Enqueue(_data.Key);
                continue;
            }
            if (_data.Value.Spotted || Time.time - _data.Value.TimeLastDetected <= parameters.TimeToLose)
            {
                _data.Value.WeakRefresh();
            }
            if (ClosestTarget == null || !Targets.ContainsKey(ClosestTarget.Target))
            {
                ClosestTarget = _data.Value;
            }
            else
            {
                float dist = (transform.position - ClosestTarget.KnownPos).sqrMagnitude;
                float newdist = (transform.position - _data.Value.KnownPos).sqrMagnitude;
                if (newdist - dist > 1)
                {
                    ClosestTarget = _data.Value;
                }
            }
        }
        while (_queue.Count > 0)
        {
            //we can't remove targets in the foreach loop, we must do it separately
            Targets.Remove(_queue.Dequeue());
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