using UnityEngine;

public class EvilDaveBT : MobCombatantBT
{
    Node AddChaseNode(Composite parent)
    {
        Node chase = parent.AddChild(new LeafNode("Chase", () =>
        {
            var _p = awarenessSystem.ClosestTarget.KnownPos;
            var _t = localMemory.GetData<Vector3>("TargetPrevPos");
            if (Vector3.Magnitude(_p - _t) > 0)
            {
                localMemory.SetData("TargetPrevPos", _p);
                actions.MoveToLocation(_p);
            }
            return NodeState.RUNNING;
        }, () =>
        {
            localMemory.SetData("TargetPrevPos", awarenessSystem.ClosestTarget.
                KnownPos);
            actions.MoveToLocation(awarenessSystem.ClosestTarget.KnownPos);
        }));

        Decorator _hastarget = chase.AddDecorator(new Decorator("Has target?", () =>
        {
            return localMemory.GetData<EntityBase>("Target") != null;
        }));
        _hastarget.MonitorValue(localMemory, "Target");

        return chase;
    }
    Node AddAttackNode(Composite _parent)
    {
        var _atk = _parent.AddChild(new LeafNode("Attack",
        () =>
        {
            actions.FaceTarget(localMemory.GetData<EntityBase>("Target").transform.position);

            return NodeState.RUNNING;
        },
        () =>
        {
            actions.Stop();
        }));
        Decorator _range = _atk.AddDecorator(new Decorator("Range Check", () =>
        {
            var t = localMemory.GetData<EntityBase>("Target");
            if (t == null)
            {
                return false;
            }
            if (Vector3.Distance(transform.position, t.transform.position) >= actions.GetEntity.CurrentWeapon.Reach)
            {
                return false;
            }
            return true;
        }));
        _range.MonitorValue(localMemory, "MyPos");
        _range.MonitorValue(localMemory, "Target");
        return _atk;
    }
}
