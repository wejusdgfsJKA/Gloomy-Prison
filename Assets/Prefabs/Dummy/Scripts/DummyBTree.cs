using UnityEngine;

public class DummyBTree : BTree
{
    protected AwarenessSystem awarenessSystem;
    protected DummyEntity dummyEntity;
    protected override void Awake()
    {
        base.Awake();
        dummyEntity = GetComponent<DummyEntity>();
        awarenessSystem = GetComponent<AwarenessSystem>();
    }
    protected override Node SetupTree()
    {
        root = new Selector("Root");
        root.AddService("Target selection", () =>
        {
            if (awarenessSystem.ClosestTarget == null)
            {
                LocalMemory.SetData<TargetData>("Target", null);
            }
            else
            {
                LocalMemory.SetData("Target", awarenessSystem.targets[awarenessSystem.ClosestTarget]);
            }
        });
        //AddAttackNode(root);

        AddChaseNode(root);

        AddIdleNode(root);

        return root;
    }
    Node AddChaseNode(Node parent)
    {
        Node ChaseNode = parent.Attach(new Node("Chase", () =>
        {
            //Debug.Log("Beggining chase");
            Vector3 pos = LocalMemory.GetData<TargetData>("Target").KnownPos;
            dummyEntity.MoveToLocation(pos);
            LocalMemory.SetData("PrevPos", pos);
            return NodeState.SUCCESS;
        },
        () =>
        {
            //Debug.Log("Chasing");
            Vector3 pos = LocalMemory.GetData<TargetData>("Target").KnownPos;
            if ((LocalMemory.GetData<Vector3>("PrevPos") - pos).sqrMagnitude > 0.01f)
            {
                dummyEntity.MoveToLocation(pos);
                LocalMemory.SetData("PrevPos", pos);
            }
            return NodeState.RUNNING;
        }));
        ChaseNode.AddDecorator("HasTarget?", () =>
        {
            try
            {
                return LocalMemory.GetData<TargetData>("Target") != null;
            }
            catch
            {
                return false;
            }
        });
        return ChaseNode;
    }
    Node AddIdleNode(Node parent)
    {
        return parent.Attach(new Node("Idle",
            () =>
            {
                dummyEntity.StopMoving();
                //dummyEntity.MoveToLocation(transform.position + transform.forward * 10);
                return NodeState.SUCCESS;
            },
            () =>
            {
                //Debug.Log("Idle");
                //maybe have him wander around, WIP
                return NodeState.RUNNING;
            }));
    }
    Node AddAttackNode(Node parent)
    {
        Node AttackNode = parent.Attach(new Node("Attack", () =>
        {
            return NodeState.SUCCESS;
        },
        () =>
        {
            dummyEntity.Swing(0);
            return NodeState.RUNNING;
        }));
        AttackNode.AddDecorator("Is target in range?", () =>
            {
                return Vector3.Distance(transform.position, LocalMemory.
                    GetData<Transform>("Target").position) <= LocalMemory.
                    GetData<float>("Reach");
            });
        return AttackNode;
    }
}
