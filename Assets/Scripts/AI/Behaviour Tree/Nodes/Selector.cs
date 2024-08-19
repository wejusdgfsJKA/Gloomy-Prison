public class Selector : Node
{
    public Selector(string Name)
    {
        this.Name = Name;
        EvaluateFunc = null;
    }
    protected override bool StopOnChildNotFailed()
    {
        return true;
    }
    protected override void OnTickedAllChildren()
    {
        LastState = Children[^1].LastState;
    }
}
