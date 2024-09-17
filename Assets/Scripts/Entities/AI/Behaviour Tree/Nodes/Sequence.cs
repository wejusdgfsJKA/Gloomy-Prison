public class Sequence : Node
{
    public Sequence(string _name) : base(_name) { }
    protected override bool StopOnChildFailed()
    {
        return true;
    }
    protected override void OnTickedAllChildren()
    {
        LastState = children[^1].LastState;
    }
}
