using System.Text;

public class Decorator : ElementBase
{
    protected System.Func<bool> OnEvaluateFn;
    public bool LastResult { get; protected set; }
    public Decorator(string Name, System.Func<bool> OnEvaluateFn)
    {
        this.Name = Name;
        this.OnEvaluateFn = OnEvaluateFn;
    }
    public bool Evaluate()
    {
        if (OnEvaluateFn != null)
        {
            LastResult = OnEvaluateFn.Invoke();
        }
        else
        {
            LastResult = false;
        }
        return LastResult;
    }
    public override void GetDebugTextInternal(StringBuilder debugTextBuilder, int indentLevel = 0)
    {
        // apply the indent
        for (int index = 0; index < indentLevel; ++index)
            debugTextBuilder.Append(' ');

        debugTextBuilder.Append($"D: {Name} [{(LastResult ? "PASS" : "FAIL")}]");
    }
}
