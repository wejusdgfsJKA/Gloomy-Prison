using UnityEngine;

public class MobCombatantBT : BTree
{
    protected MobCombatantActions actions;
    protected AwarenessSystem awarenessSystem;
    protected override Composite SetupTree()
    {
        actions = GetComponent<MobCombatantActions>();
        awarenessSystem = GetComponent<AwarenessSystem>();

        root = new Selector("Root");
        AddUpdateService();

        root.AddChild(CreateCombatSubtree());

        return root;
    }
    protected virtual void AddUpdateService()
    {
        root.AddService(new Service("UpdateData", () =>
        {
            localMemory.SetData("MyPos", transform.position);

            var t = awarenessSystem.ClosestTarget;
            if (t != null)
            {
                localMemory.SetData("Target", awarenessSystem.ClosestTarget.Entity);
                localMemory.SetData("TargetPos", awarenessSystem.ClosestTarget.KnownPos);
            }
            else localMemory.SetData<EntityBase>("Target", null);
        }));
    }
    protected virtual Composite CreateCombatSubtree()
    {
        Selector _combatSubtree = new Selector("CombatSubtree");

        _combatSubtree.AddDecorator(new Decorator("HasTarget", () =>
        {
            return localMemory.GetData<EntityBase>("Target") != null;
        })).MonitorValue(localMemory, "Target");

        _combatSubtree.AddChild(CreateChaseNode());

        return _combatSubtree;
    }
    protected LeafNode CreateChaseNode()
    {
        return new LeafNode("ChaseNode", () =>
        {
            var _pos = localMemory.GetData<Vector3>("TargetPos");
            if (Vector3.SqrMagnitude(_pos - actions.Destination) > 0.1f)
            {
                actions.MoveToLocation(_pos);
            }
            return NodeState.RUNNING;
        }, () =>
        {
            actions.MoveToLocation(localMemory.GetData<Vector3>("TargetPos"));
        });
    }
}
