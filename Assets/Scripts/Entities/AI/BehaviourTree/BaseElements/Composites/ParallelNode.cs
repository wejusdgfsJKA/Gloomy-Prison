public class ParallelNode : Composite
{
    public ParallelNode(string name, Node leftChild, Node rightChild) : base(name)
    {
        children.Add(leftChild);
        children.Add(rightChild);
    }
    public override bool Evaluate()
    {
        if (base.Evaluate())
        {
            children[0].Evaluate();
            state = children[0].State;
            if (state != NodeState.FAILURE)
            {
                children[1].Evaluate();
            }
            return true;
        }
        return false;
    }
    public override void NewLeftmost(Node _child)
    {

    }

    public override void UpdateLeftmost()
    {

    }
}
