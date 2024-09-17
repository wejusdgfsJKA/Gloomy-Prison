public class Selector : Node
{
    public Selector(string _name) : base(_name) { }
    protected override bool StopOnChildNotFailed()
    {
        return true;
    }
    protected override void OnTickedAllChildren()
    {
        LastState = children[^1].LastState;
    }
}
