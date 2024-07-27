using System.Text;

public abstract class ElementBase
{
    public string Name { get; protected set; } = "No Name";
    public string GetDebugText(int indentLevel = 0)
    {
        StringBuilder debugTextBuilder = new StringBuilder();

        GetDebugTextInternal(debugTextBuilder, indentLevel);

        return debugTextBuilder.ToString();
    }
    public abstract void GetDebugTextInternal(StringBuilder debugTextBuilder, int indentLevel = 0);

}
