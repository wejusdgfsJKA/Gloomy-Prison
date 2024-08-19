using UnityEngine;

public class MonsterBTree : BTree
{
    protected AwarenessSystem awarenessSystem;
    protected DummyEntity dummyEntity;
    public float MaxThreats;//how many threats is this entity capable of accounting for
    protected override void Awake()
    {
        base.Awake();
        dummyEntity = GetComponent<DummyEntity>();
        awarenessSystem = GetComponent<AwarenessSystem>();
    }
    protected override Node SetupTree()
    {
        LocalMemory.SetData<float>("Mood", .5f);
        root = new Selector("Root");
        root.AddService("Target selection", () =>
        {
            if (awarenessSystem.ClosestTarget == null)
            {
                LocalMemory.SetData<TargetData>("Target", null);
            }
            else
            {
                LocalMemory.SetData("Target", awarenessSystem.ClosestTarget);
            }
        });
        root.AddService("Mood", () =>
        {
            //if scared, will dodge rather than block
            if (dummyEntity.Damaged)
            {
                dummyEntity.Damaged = false;
                float newmood = Mathf.Clamp(LocalMemory.GetData<float>("Mood") - .1f, 0, 1);
                LocalMemory.SetData("Mood", newmood);
            }
        });

        //add combat subtrees

        AddDefenseSubtree(root);

        AddOffenseSubtree(root);

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
                dummyEntity.Feint();
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
    Node AddDefenseSubtree(Node parent)
    {
        Node node = parent.Attach(new Selector("Defense Subtree"));
        node.AddDecorator("Under direct threat?", () =>
        {
            //are we under threat from our closest enemy?
            //ye ye ik it's crap, WIP
            var t = awarenessSystem.ClosestTarget;
            if (t != null)
            {
                return awarenessSystem.ClosestTarget.data.IsThreat(transform);
            }
            return false;
        });
        node.AddService("Generate response", () =>
        {
            LocalMemory.SetData<float>("Defensive response", Random.value);
        });
        AddDodgeNode(node).AddDecorator("Number gauge", () =>
        {
            return LocalMemory.GetData<float>("Defensive response") > LocalMemory.GetData<float>("Mood");
        });
        AddBlockNode(node);
        return node;
    }
    Node AddDodgeNode(Node parent)
    {
        Node node = parent.Attach(new Node("Dodge", () =>
        {
            dummyEntity.Dodge(-transform.forward);
            return NodeState.SUCCESS;
        }));
        return node;
    }
    Node AddBlockNode(Node parent)
    {
        Node node = parent.Attach(new Node("Block", () =>
        {
            dummyEntity.StopMoving();
            dummyEntity.FaceTarget(awarenessSystem.ClosestTarget.KnownPos);
            dummyEntity.AimWeapon(awarenessSystem.ClosestTarget.data.GetWeaponCenter().position);
            dummyEntity.BeginBlocking();
            return NodeState.SUCCESS;
        },
        () =>
        {
            dummyEntity.FaceTarget(awarenessSystem.ClosestTarget.KnownPos);
            if (awarenessSystem.ClosestTarget.data.WeaponActive())
            {
                dummyEntity.AimWeapon(awarenessSystem.ClosestTarget.data.GetWeaponCenter().position);
            }
            if (dummyEntity.ReturnCurrentState() != EntityState.Blocking)
            {
                dummyEntity.BeginBlocking();
            }
            return NodeState.RUNNING;
        }));
        return node;
    }
    Node AddOffenseSubtree(Node parent)
    {
        Node node = parent.Attach(new Selector("Offense Subtree"));
        node.AddDecorator("Is target in range?", () =>
        {
            return awarenessSystem.ClosestTarget != null && Vector3.
            Distance(transform.position, LocalMemory.GetData<TargetData>("Target").
            KnownPos) <= dummyEntity.GetReach();
        });
        AddSwingNode(node);
        return node;
    }
    Node AddSwingNode(Node parent)
    {
        Node node;
        node = parent.Attach(new Node("Strike", () =>
        {
            dummyEntity.StopMoving();
            dummyEntity.FaceTarget(awarenessSystem.ClosestTarget.KnownPos);
            dummyEntity.AimWeapon(awarenessSystem.ClosestTarget.KnownPos);
            return NodeState.RUNNING;
        },
        () =>
        {
            dummyEntity.FaceTarget(awarenessSystem.ClosestTarget.KnownPos);
            dummyEntity.AimWeapon(awarenessSystem.ClosestTarget.KnownPos);
            dummyEntity.Swing();
            return NodeState.RUNNING;
        }));
        return node;
    }
    Node AddPushNode(Node parent)
    {
        Node node = parent.Attach(new Node("Push", () =>
        {
            return NodeState.RUNNING;
        },
        () =>
        {
            dummyEntity.AimWeapon(awarenessSystem.ClosestTarget.KnownPos);
            dummyEntity.FaceTarget(awarenessSystem.ClosestTarget.KnownPos);
            return NodeState.RUNNING;
        }));
        return node;
    }
}
