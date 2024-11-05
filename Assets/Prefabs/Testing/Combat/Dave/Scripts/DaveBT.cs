using UnityEngine;

public class DaveBT : BTree
{
    protected MobCombatantActions actions;
    protected AwarenessSystem awarenessSystem;
    protected override Composite SetupTree()
    {
        localMemory.SetData<EntityBase>("Target", null);
        localMemory.SetData<EntityBase>("TargetPrevPos", null);
        localMemory.SetData("MyPos", transform.position);
        actions = GetComponent<MobCombatantActions>();
        awarenessSystem = GetComponent<AwarenessSystem>();
        root = new Selector("Root");
        AddAttackNode(root);
        AddDefenseNode(root);
        AddChaseNode(root);
        root.AddService(new Service("Update", () =>
        {
            localMemory.SetData("MyPos", transform.position);

            var t = awarenessSystem.ClosestTarget;
            if (t != null)
            {
                localMemory.SetData("Target", awarenessSystem.ClosestTarget.Entity);
            }
            else localMemory.SetData<EntityBase>("Target", null);
        }));

        return root;
    }
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
            var _target = localMemory.GetData<EntityBase>("Target");
            actions.FaceTarget(_target.transform.position);

            var _wpnstate = actions.GetEntity.CurrentWeapon.CurrentState;

            if (_target.CurrentWeapon.CurrentState == Weapon.State.Release)
            {
                switch (_wpnstate)
                {
                    case Weapon.State.Idle:
                        actions.GetEntity.CurrentWeapon.Block(true);
                        break;
                    case Weapon.State.Windup:
                        actions.PerformAttack(_target.CurrentWeapon.LastAttack.AttackType);
                        break;
                    default:
                        break;
                }
                if (_wpnstate != Weapon.State.Windup && _wpnstate != Weapon.State.Release)
                {
                    int i = Random.Range(0, 2);
                    Attack.Type _type = Attack.Type.Kick;
                    switch (i)
                    {
                        case 0:
                            _type = Attack.Type.Light;
                            break;
                        case 1:
                            _type = Attack.Type.Thrust;
                            break;
                        case 2:
                            _type = Attack.Type.Overhead;
                            break;
                    }
                    actions.PerformAttack(_type);
                }
                return NodeState.RUNNING;
            }
            if (_wpnstate != Weapon.State.Windup && _wpnstate != Weapon.State.Release)
            {
                int i = Random.Range(0, 2);
                Attack.Type _type = Attack.Type.Kick;
                switch (i)
                {
                    case 0:
                        _type = Attack.Type.Light;
                        break;
                    case 1:
                        _type = Attack.Type.Thrust;
                        break;
                    case 2:
                        _type = Attack.Type.Overhead;
                        break;
                }
                actions.PerformAttack(_type);
            }
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
    Node AddDefenseNode(Composite _parent)
    {
        var _atk = _parent.AddChild(new LeafNode("Defense",
        () =>
        {
            actions.FaceTarget(localMemory.GetData<EntityBase>("Target").transform.position);

            var _pos = awarenessSystem.ClosestTarget.KnownPos;
            var _prev = localMemory.GetData<Vector3>("TargetPrevPos");
            if (Vector3.Magnitude(_pos - _prev) > 0)
            {
                localMemory.SetData("TargetPrevPos", _pos);
                actions.MoveToLocation(_pos, false);
            }

            var _target = localMemory.GetData<EntityBase>("Target");
            var _weapon = actions.GetEntity.CurrentWeapon;
            if (_target.CurrentWeapon.CurrentState == Weapon.State.Release)
            {
                _weapon.Block(true);
            }
            else if (_weapon.CurrentState == Weapon.State.Blocking)
            {
                _weapon.Block(false);
            }

            return NodeState.RUNNING;
        },
        () =>
        {
            localMemory.SetData("TargetPrevPos", awarenessSystem.ClosestTarget.
                KnownPos);
            actions.MoveToLocation(awarenessSystem.ClosestTarget.KnownPos);
        },
        () =>
        {
            actions.GetEntity.CurrentWeapon.Block(false);
        }));
        Decorator _range = _atk.AddDecorator(new Decorator("Range Check", () =>
        {
            var t = localMemory.GetData<EntityBase>("Target");
            if (t == null)
            {
                return false;
            }
            if (Vector3.Distance(transform.position, t.transform.position) >= t.CurrentWeapon.Reach)
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
