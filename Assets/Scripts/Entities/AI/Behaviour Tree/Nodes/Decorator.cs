using System.Text;

public class Decorator : ElementBase
{
    protected System.Func<bool> onEvaluateFn;
    public bool LastResult { get; protected set; }
    public Decorator(string _name, System.Func<bool> _onevaluate)
    {
        Name = _name;
        onEvaluateFn = _onevaluate;
    }
    public bool Evaluate()
    {
        if (onEvaluateFn != null)
        {
            LastResult = onEvaluateFn.Invoke();
        }
        else
        {
            LastResult = false;
        }
        return LastResult;
    }
    public override void GetDebugTextInternal(StringBuilder _debug, int _indentlevel = 0)
    {
        // apply the indent
        for (int _index = 0; _index < _indentlevel; ++_index)
        {
            _debug.Append(' ');
        }
        _debug.Append($"D: {Name} [{(LastResult ? "PASS" : "FAIL")}]");
    }
}
