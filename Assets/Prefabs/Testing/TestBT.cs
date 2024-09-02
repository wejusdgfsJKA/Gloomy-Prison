using UnityEngine;

public class TestBT : BTree
{
    public bool b1 = true, b2 = true, b3 = true, b4 = true;
    public float f;
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
            if (f < 50)
            {
                f++;
                return NodeState.RUNNING;
            }
            return NodeState.SUCCESS;
        },
        () =>
        {
            Debug.Log("Exited node 1");
        })).AddDecorator("bool1", () => { return b1; });
        root.Attach(new Node("Node 2", () =>
        {
            Debug.Log("Entered node 2");
            return NodeState.SUCCESS;
        },
        () =>
        {
            if (f < 100)
            {
                f++;
                return NodeState.RUNNING;
            }
            return NodeState.SUCCESS;
        },
        () =>
        {
            Debug.Log("Exited node 2");
        })).AddDecorator("bool2", () => { return b2; });
        root.Attach(new Node("Node 3", () =>
        {
            Debug.Log("Entered node 3");
            return NodeState.SUCCESS;
        },
        () =>
        {
            if (f > 50)
            {
                f--;
                return NodeState.RUNNING;
            }
            return NodeState.SUCCESS;
        },
        () =>
        {
            Debug.Log("Exited node 3");
        })).AddDecorator("bool3", () => { return b3; });
        root.Attach(new Node("Node 4", () =>
        {
            Debug.Log("Entered node 4");
            return NodeState.SUCCESS;
        },
        () =>
        {
            if (f > 0)
            {
                f--;
                return NodeState.RUNNING;
            }
            return NodeState.SUCCESS;
        },
        () =>
        {
            Debug.Log("Exited node 4");
        })).AddDecorator("bool4", () => { return b4; });
        return root;
    }
}
