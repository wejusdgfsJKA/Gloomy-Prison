using System.Collections;
using UnityEngine;

public abstract class BTree : MonoBehaviour
{
    [SerializeField] protected float updateinterval = .1f;
    public bool ShouldRun = true;
    protected Node root = null;
    protected Coroutine coroutine;
    protected BlackBoard localMemory = null;
    protected WaitForSeconds waitInterval;
    protected WaitUntil waitForPermission;
    protected virtual void Awake()
    {
        localMemory = new BlackBoard();
        waitInterval = new(updateinterval);
        waitForPermission = new(() => { return ShouldRun; });
        root = SetupTree();
    }
    protected virtual void OnEnable()
    {
        localMemory.SetData<TargetData>("Target", null);
        ShouldRun = true;
        coroutine = StartCoroutine(UpdateLoop());
    }
    protected void OnDisable()
    {
        if (coroutine != null)
        {
            StopCoroutine(coroutine);
        }
    }
    protected IEnumerator UpdateLoop()
    {
        yield return new WaitUntil(new(() => { return localMemory != null; }));
        while (true)
        {
            yield return waitInterval;
            yield return waitForPermission;
            root?.Evaluate();
        }
    }
    protected abstract Node SetupTree();
    public string GetDebugText()
    {
        return root.GetDebugText();
    }
}