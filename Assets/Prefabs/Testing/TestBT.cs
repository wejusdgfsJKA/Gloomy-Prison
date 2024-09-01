using UnityEngine;

public class TestBT : BTree
{
    public bool b1, b2;
    protected override Node SetupTree()
    {
        root = new Selector("Root");
        root.Attach(new Node("Node 1", () =>
        {
            Debug.Log("Entered node 1");
            return NodeState.SUCCESS;
        },
        () =>
        {
            return NodeState.SUCCESS;
        },
        () =>
        {
            Debug.Log("Exited node 1");
        }));
        root.Attach(new Node("Node 2", () =>
        {
            Debug.Log("Entered node 2");
            return NodeState.SUCCESS;
        },
        () =>
        {
            return NodeState.SUCCESS;
        },
        () =>
        {
            Debug.Log("Exited node 2");
        }));
        root.Attach(new Node("Node 3", () =>
        {
            Debug.Log("Entered node 3");
            return NodeState.SUCCESS;
        },
        () =>
        {
            return NodeState.SUCCESS;
        },
        () =>
        {
            Debug.Log("Exited node 3");
        }));
        return root;
    }
}
