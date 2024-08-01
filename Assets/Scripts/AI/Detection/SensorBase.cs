using System.Collections;
using UnityEditor;
using UnityEngine;
public abstract class SensorBase : MonoBehaviour
{
    [SerializeField]
    protected AwarenessSystem awarenessSystem;
    protected Coroutine coroutine;
    protected WaitForSeconds wait;
    [SerializeField]
    protected float UpdateInterval = 0.1f;
    [SerializeField]
    protected float range = 30;
    [SerializeField]
    protected bool ShowDebug;
    protected void Awake()
    {
        wait = new WaitForSeconds(UpdateInterval);
        awarenessSystem = GetComponentInParent<AwarenessSystem>();
    }
    protected virtual void OnEnable()
    {
        coroutine = StartCoroutine(enumerator());
    }
    protected virtual void OnDisable()
    {
        if (coroutine != null)
        {
            StopCoroutine(coroutine);
        }
    }
    protected IEnumerator enumerator()
    {
        while (true)
        {
            yield return wait;
            Detect();
        }
    }
    protected abstract void Detect();
    //the following are just getters for the editor scripts
    public float GetRange()
    {
        return range;
    }
    public bool GetShowDebug()
    {
        return ShowDebug;
    }
}
#if UNITY_EDITOR
[CustomEditor(typeof(SensorBase), true)]
public class SensorDebug : Editor
{
    protected virtual void OnSceneGUI()
    {
        SensorBase sensor = (SensorBase)target;
        if (sensor.GetShowDebug())
        {
            Handles.DrawWireArc(sensor.transform.position, Vector3.up,
            sensor.transform.forward, 360, sensor.GetRange());
        }
    }
}
#endif