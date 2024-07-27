using UnityEditor;
using UnityEngine;

public class VisionSensor : SensorBase
{
    [SerializeField]
    protected LayerMask ObstructionMask;
    [SerializeField]
    protected float VisionConeAngle;
    [SerializeField]
    protected Transform eye;
    protected override void Detect()
    {
        for (int i = 0; i < DetectionManager.instance.teams.Length; i++)
        {
            if (DetectionManager.instance.teams[i] != transform.root.
                gameObject.layer.ToString())
            {
                //this is an enemy team
                //Debug.Log("Checking team " + DetectionManager.instance.teams[i]);
                var team = DetectionManager.instance.targets[DetectionManager.
                    instance.teams[i]];
                foreach (Transform entity in team)
                {
                    //Debug.Log(entity);
                    if (CanSee(entity))
                    {
                        //we can see this entity
                        awarenessSystem.ReportHasSeen(entity);
                    }
                }
            }
        }
    }
    protected bool CanSee(Transform target)
    {
        Vector3 VectorToTarget = target.position - transform.position;
        if (VectorToTarget.sqrMagnitude > range * range)
        {
            //the target is out of range
            //Debug.DrawLine(transform.position, target.position, Color.yellow, 10);
            return false;
        }
        VectorToTarget.Normalize();
        if (Vector3.Dot(VectorToTarget, eye.forward) < Mathf.Cos(Mathf.Deg2Rad * VisionConeAngle))
        {
            //the target is not in our field of view
            //Debug.DrawLine(transform.position, target.position, Color.magenta, 10);
            return false;
        }
        if (Physics.Linecast(transform.position, target.position, ObstructionMask))
        {
            //the target is obstructed
            //Debug.DrawLine(transform.position, target.position, Color.red, 10);
            return false;
        }
        //Debug.DrawLine(transform.position, target.position, Color.green, 10);
        return true;
    }
    public float GetAngle()
    {
        return VisionConeAngle;
    }
}
#if UNITY_EDITOR
[CustomEditor(typeof(VisionSensor))]
public class VisionDebug : Editor
{
    private void OnSceneGUI()
    {
        VisionSensor sensor = (VisionSensor)target;
        Handles.color = Color.yellow;
        Handles.DrawWireArc(sensor.transform.position, Vector3.up,
            sensor.transform.forward, 360, sensor.GetRange());
        float angle = Mathf.Deg2Rad * (90 - sensor.GetAngle());
        Vector3 p1 = sensor.transform.position + Mathf.Sin(angle) * sensor.GetRange()
            * sensor.transform.forward + Mathf.Cos(angle) * sensor.GetRange() *
            sensor.transform.right;
        Handles.DrawLine(sensor.transform.position, p1);
        Vector3 p2 = sensor.transform.position + Mathf.Sin(angle) * sensor.GetRange()
            * sensor.transform.forward - Mathf.Cos(angle) * sensor.GetRange() *
            sensor.transform.right;
        Handles.DrawLine(sensor.transform.position, p2);
    }
}
#endif