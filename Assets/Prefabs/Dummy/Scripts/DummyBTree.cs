using UnityEngine;

public class DummyBTree : BTree
{
    protected AwarenessSystem awarenessSystem;
    protected DummyEntity dummyEntity;
    protected void Awake()
    {
        dummyEntity = GetComponent<DummyEntity>();
        awarenessSystem = GetComponent<AwarenessSystem>();
    }
    protected override void OnEnable()
    {
        base.OnEnable();
        LocalMemory?.SetData<Transform>("Target", null);
        SetReach(reach);
    }
    protected override Node SetupTree()
    {
        root = new Selector("Root");
        root.AddService(new Service("Has target?",
            () =>
            {
                if (awarenessSystem.target != null && awarenessSystem.target.gameObject.activeSelf)
                {
                    LocalMemory.SetData<Transform>("Target", awarenessSystem.target);
                    //Debug.DrawLine(transform.position, awarenessSystem.target.position, Color.red, 10);
                }
                else
                {
                    LocalMemory.SetData<Transform>("Target", null);
                }
            }));

        //AddAttackNode(root);

        AddChaseNode(root);

        AddIdleNode(root);

        return root;
    }
    public void SetReach(float reach)
    {
        LocalMemory.SetData("Reach", reach);
    }
    Node AddChaseNode(Node parent)
    {
        Node ChaseNode = parent.Attach(new Node("Chase", () =>
        {
            //Debug.Log("Beggining chase");
            dummyEntity.MoveToLocation(LocalMemory.
                GetData<Transform>("Target").position);
            LocalMemory.SetData("PrevPos", LocalMemory.
                GetData<Transform>("Target").position);
            return NodeState.SUCCESS;
        },
        () =>
        {
            //Debug.Log("Chasing");
            if ((LocalMemory.GetData<Vector3>("PrevPos") - LocalMemory.
                GetData<Transform>("Target").position).sqrMagnitude > 0.01f)
            {
                dummyEntity.MoveToLocation(LocalMemory.
                    GetData<Transform>("Target").position);
                LocalMemory.SetData("PrevPos", LocalMemory.
                    GetData<Transform>("Target").position);
            }
            return NodeState.RUNNING;
        }));
        ChaseNode.AddDecorator("HasTarget?", () =>
        {
            return LocalMemory.GetData<Transform>("Target") != null;
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
