using UnityEngine;

public class TestBT : BTree
{
    public float a = 0;
    public bool B11 = true, B12 = true, B21 = true, B22 = true, B1 = true, B2 = true;
    bool b11 = true, b12 = true, b21 = true, b22 = true, b1 = true, b2 = true;
    protected override Composite SetupTree()
    {
        localMemory.SetData("1-1", true);
        localMemory.SetData("1-2", true);
        localMemory.SetData("2-1", true);
        localMemory.SetData("2-2", true);
        localMemory.SetData("1", true);
        localMemory.SetData("2", true);
        root = new Selector("Root");
        Selector sel1 = AddSelParent(root, "1");
        AddKid(sel1, "1-1");
        AddKid(sel1, "1-2");
        Sequence seq2 = AddSeqParent(root, "2");
        AddKid(seq2, "2-1");
        AddKid(seq2, "2-2");
        return root;
    }
    private void Update()
    {
        if (b1 != B1)
        {
            b1 = B1;
            localMemory.SetData("1", b1);
        }
        if (b2 != B2)
        {
            b2 = B2;
            localMemory.SetData("2", b2);
        }
        if (B11 != b11)
        {
            localMemory.SetData("1-1", B11);
            b11 = B11;
        }
        if (B12 != b12)
        {
            localMemory.SetData("1-2", B12);
            b12 = B12;
        }
        if (B21 != b21)
        {
            localMemory.SetData("2-1", B21);
            b21 = B21;
        }
        if (B22 != b22)
        {
            localMemory.SetData("2-2", B22);
            b22 = B22;
        }
    }
    protected Sequence AddSeqParent(Composite _parent, string _name)
    {
        var _node = _parent.AddChild(new Sequence(_name));

        var _decorator = _node.AddDecorator(new Decorator("Bool check " + _name, () =>
        {
            return (bool)localMemory.GetData(_name).Value;
        }));

        _decorator.MonitorValue(localMemory, _name);
        return (Sequence)_node;
    }
    protected Selector AddSelParent(Composite _parent, string _name)
    {
        var _node = _parent.AddChild(new Selector(_name));

        var _decorator = _node.AddDecorator(new Decorator("Bool check " + _name, () =>
        {
            return (bool)localMemory.GetData(_name).Value;
        }));

        _decorator.MonitorValue(localMemory, _name);
        return (Selector)_node;
    }
    protected void AddKid(Composite _parent, string _name)
    {
        var _node = _parent.AddChild(new LeafNode(_name, () =>
        {
            a++;
            if (a >= 3)
            {
                Debug.Log(_name + " success" + " on A = " + a);
                return NodeState.SUCCESS;
            }
            Debug.Log("Running " + _name + " on A = " + a);
            return NodeState.RUNNING;
        }, () =>
        {
            Debug.Log("Entered " + _name + " on A = " + a);
        },
        () =>
        {
            a = 0;
            Debug.Log("Exited " + _name + " on A = " + a);
        }));

        var _decorator = _node.AddDecorator(new Decorator("Bool check " + _name, () =>
        {
            return (bool)localMemory.GetData(_name).Value;
        }));
        _decorator.MonitorValue(localMemory, _name);
    }
}
