using System.Collections;
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
    protected float range;
    protected void Awake()
    {
        wait = new WaitForSeconds(UpdateInterval);
    }
    protected virtual void OnEnable()
    {
        coroutine = StartCoroutine(enumerator());
    }
    public float GetRange()
    {
        return range;
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
}