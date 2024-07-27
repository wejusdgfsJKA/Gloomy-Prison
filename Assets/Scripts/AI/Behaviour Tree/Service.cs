using System.Text;

public class Service : ElementBase
{
    protected System.Action OnEvaluateFn;
    public Service(string Name, System.Action OnEvaluateFn)
    {
        this.Name = Name;
        this.OnEvaluateFn = OnEvaluateFn;
    }
    public void Evaluate()
    {
        if (OnEvaluateFn != null)
        {
            OnEvaluateFn();
        }
    }
    public override void GetDebugTextInternal(StringBuilder debugTextBuilder, int indentLevel = 0)
    {
        // apply the indent
        for (int index = 0; index < indentLevel; ++index)
            debugTextBuilder.Append(' ');

        debugTextBuilder.Append($"S: {Name}");
    }
}
