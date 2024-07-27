public class Sequence : Node
{
    public Sequence(string name) : base(name) { }
    protected override bool StopOnChildFailed()
    {
        return true;
    }
    protected override void OnTickedAllChildren()
    {
        LastState = Children[^1].LastState;
    }
}
