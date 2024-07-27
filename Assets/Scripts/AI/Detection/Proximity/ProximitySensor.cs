using UnityEditor;
using UnityEngine;
public class ProximitySensor : SensorBase
{
    protected override void Detect()
    {
        //simple distance check
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
                    if (Vector3.Distance(transform.position, entity.position) <= range)
                    {
                        //we can see this entity
                        awarenessSystem.ReportInProximity(entity);
                    }
                }
            }
        }
    }
}
#if UNITY_EDITOR
[CustomEditor(typeof(ProximitySensor))]
public class ProximityDebug : Editor
{
    private void OnSceneGUI()
    {
        ProximitySensor sensor = (ProximitySensor)target;
        Handles.color = Color.cyan;
        Handles.DrawWireArc(sensor.transform.position, Vector3.up, sensor.transform.forward, 360, sensor.GetRange());
    }
}
#endif