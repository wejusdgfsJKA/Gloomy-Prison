using UnityEditor;
using UnityEngine;
public class HearingSensor : MonoBehaviour
{
    [SerializeField]
    protected AwarenessSystem awarenessSystem;
    [SerializeField]
    protected float range;
    protected void OnEnable()
    {
        DetectionManager.instance.RegisterListener(this);
    }
    protected void OnDisable()
    {
        DetectionManager.instance.DeRegisterListener(this);
    }
    public void Hear(Sound sound)
    {
        if (CanHear(sound))
        {
            awarenessSystem.ReportHasHeard(sound);
        }
    }
    protected bool CanHear(Sound sound)
    {
        return Vector3.Distance(transform.position, sound.data.source.position) <= range;
    }
    public float GetRange()
    {
        return range;
    }
}
#if UNITY_EDITOR
[CustomEditor(typeof(HearingSensor))]
public class HearingDebug : Editor
{
    private void OnSceneGUI()
    {
        HearingSensor sensor = (HearingSensor)target;
        Handles.color = Color.magenta;
        Handles.DrawWireArc(sensor.transform.position, Vector3.up,
            sensor.transform.forward, 360, sensor.GetRange());
    }
}
#endif