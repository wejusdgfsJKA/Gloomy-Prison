using System.Collections;
using UnityEngine;

public abstract class BTree : MonoBehaviour
{
    public bool shouldrun = true;
    protected Node root = null;
    protected Coroutine coroutine;
    [SerializeField]
    protected float updateinterval = .1f;
    protected BlackBoard LocalMemory = null;
    protected virtual void Awake()
    {
        LocalMemory = new BlackBoard();
        root = SetupTree();
    }
    protected virtual void OnEnable()
    {
        LocalMemory.SetData<TargetData>("Target", null);
        shouldrun = true;
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
        WaitForSeconds waitinterval = new(updateinterval);
        WaitUntil waitforpermission = new(() => { return shouldrun; });
        yield return new WaitUntil(new(() => { return LocalMemory != null; }));
        while (true)
        {
            yield return waitinterval;
            yield return waitforpermission;
            root?.Evaluate();
        }
    }
    protected abstract Node SetupTree();
    public string GetDebugText()
    {
        return root.
            GetDebugText();
    }
}
