using System.Collections.Generic;
using System.Text;
using UnityEngine;

public abstract class Composite : Node
{
    protected List<Node> children = new();
    protected int leftmost;
    public int Leftmost
    {
        get
        {
            return leftmost;
        }
    }
    public Composite(string _name) : base(_name)
    {
        //reset our starting point everytime we enter
        onEnter += () =>
        {
            Debug.Log("Entered composite " + _name);
            leftmost = 0;
        };
    }
    public Node AddChild(Node _node)
    {
        _node.Parent = this;
        _node.IndexInParent = children.Count;
        children.Add(_node);
        return _node;
    }
    public abstract void NewLeftmost(int _index);
    public abstract void UpdateLeftmost();
    public override void GetDebugTextInternal(StringBuilder _debug, int _indentlevel = 0)
    {
        base.GetDebugTextInternal(_debug, _indentlevel);
        _debug.AppendLine();
        _debug.Append("Leftmost: " + leftmost);
        foreach (var _child in children)
        {
            _debug.AppendLine();
            _child.GetDebugTextInternal(_debug, _indentlevel + 2);
        }
    }
}
