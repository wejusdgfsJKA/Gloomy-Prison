using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
public class AwarenessSystem : MonoBehaviour
{
    [SerializeField]
    protected DetectionParameters parameters;
    protected WaitForSeconds wait;
    protected Coroutine coroutine;
    [SerializeField]
    protected Transform eye;
    [SerializeField]
    protected bool ShowDebug;
    public Dictionary<Transform, TargetData> targets { get; protected set; } = new();
    public TargetData ClosestTarget { get; protected set; }
    protected void Awake()
    {
        wait = new WaitForSeconds(parameters.CheckInterval);
    }
    protected void OnEnable()
    {
        DetectionManager.instance.RegisterListener(this);
        coroutine = StartCoroutine(enumerator());
    }
    protected void OnDisable()
    {
        DetectionManager.instance.DeRegisterListener(this);
        if (coroutine != null)
        {
            StopCoroutine(coroutine);
        }
        targets.Clear();
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
        if (DetectionManager.instance.teams != null)
        {
            for (int i = 0; i < DetectionManager.instance.teams.Length; i++)
            {
                if (DetectionManager.instance.teams[i] != transform.root.
                    gameObject.layer.ToString())
                {
                    //this is an enemy team
                    var team = DetectionManager.instance.targets[DetectionManager.
                        instance.teams[i]];
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
        foreach (var data in targets)
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
            if (data.Value.spotted || Time.time - data.Value.TimeLastDetected <= parameters.TimeToLose)
            {
                data.Value.WeakRefresh();
            }
            if (ShowDebug)
            {
                Debug.DrawLine(transform.position, data.Value.KnownPos, Color.blue, 1);
                //Debug.DrawLine(transform.position, data.Value.target.position, Color.cyan, 1);
            }
            if (ClosestTarget == null || !targets.ContainsKey(ClosestTarget.target))
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
            targets.Remove(queue.Dequeue());
        }
    }
    protected void HasDetected(Transform t)
    {
        //we have detected this target
        try
        {
            targets[t].Refresh();
        }
        catch (KeyNotFoundException)
        {
            targets.Add(t, new TargetData(t));
        }
    }
    public void Hear(Sound sound)
    {
        if (CanHear(sound))
        {
            HasDetected(sound.data.source);
        }
    }
    protected bool CanHear(Sound sound)
    {
        //can we hear a given sound? WIP
        return Vector3.Distance(transform.position, sound.data.source.position) <= parameters.HearingRange;
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
    //the following are just getters for the editor scripts
    public float GetProximityDetectionRange()
    {
        return parameters.ProximityDetectionRange;
    }
    public float GetSightRange()
    {
        return parameters.SightRange;
    }
    public float GetSightAngle()
    {
        return parameters.SightAngle;
    }
    public float GetHearingRange()
    {
        return parameters.HearingRange;
    }
    public bool GetShowDebug()
    {
        return ShowDebug;
    }
}
#if UNITY_EDITOR
[CustomEditor(typeof(AwarenessSystem))]
public class AwarenessDebug : Editor
{
    private void OnSceneGUI()
    {
        AwarenessSystem awarenessSystem = (AwarenessSystem)target;
        if (awarenessSystem.GetShowDebug())
        {
            //show the targets
            {
                Handles.color = Color.blue;
                foreach (var pair in awarenessSystem.targets)
                {
                    Handles.DrawLine(awarenessSystem.transform.position, pair.Value.KnownPos);
                }
            }
            //show proximity detection
            {
                Handles.color = Color.cyan;
                Handles.DrawWireArc(awarenessSystem.transform.position, Vector3.up,
                    awarenessSystem.transform.forward, 360, awarenessSystem.GetProximityDetectionRange());
            }
            //show visual detection
            {
                Handles.color = Color.yellow;
                Handles.DrawWireArc(awarenessSystem.transform.position, Vector3.up,
                    awarenessSystem.transform.forward, 360, awarenessSystem.
                    GetSightRange());
                float angle = Mathf.Deg2Rad * (90 - awarenessSystem.GetSightAngle());
                Vector3 p1 = awarenessSystem.transform.position + Mathf.Sin(angle) *
                    awarenessSystem.GetSightRange() * awarenessSystem.transform.
                    forward + Mathf.Cos(angle) * awarenessSystem.GetSightRange() *
                    awarenessSystem.transform.right;
                Handles.DrawLine(awarenessSystem.transform.position, p1);
                p1 = awarenessSystem.transform.position + Mathf.Sin(angle) *
                    awarenessSystem.GetSightRange() * awarenessSystem.transform.
                    forward - Mathf.Cos(angle) * awarenessSystem.GetSightRange() *
                    awarenessSystem.transform.right;
                Handles.DrawLine(awarenessSystem.transform.position, p1);
            }
            //show hearing
            {
                Handles.color = Color.magenta;
                Handles.DrawWireArc(awarenessSystem.transform.position, Vector3.up,
                    awarenessSystem.transform.forward, 360, awarenessSystem.GetHearingRange());
            }
        }
    }
}
#endif