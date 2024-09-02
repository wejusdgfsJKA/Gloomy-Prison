using System.Collections.Generic;
using System.Text;
public enum NodeState
{
    UNKNOWN,
    RUNNING,
    SUCCESS,
    FAILURE
}

public class Node : ElementBase
{
    public NodeState LastState { get; protected set; } = NodeState.UNKNOWN;
    public Node Parent { get; protected set; }
    protected List<Node> Children = new();
    protected List<Service> Services = new();
    protected List<Decorator> Decorators = new();
    protected System.Func<NodeState> EnterFunc;
    protected System.Func<NodeState> EvaluateFunc;
    protected System.Action OnExitFunc;
    public bool DecoratorsPermitRunning { get; protected set; } = true;
    public Node(string Name = "No Name", System.Func<NodeState> EnterFunc = null, System.Func<NodeState> EvaluateFunc = null, System.Action onExitFunc = null)
    {
        this.Name = Name;
        this.EnterFunc = EnterFunc;
        this.EvaluateFunc = EvaluateFunc;
        this.OnExitFunc = onExitFunc;
    }
    public Node Attach(Node node)
    {
        node.Parent = this;
        Children.Add(node);
        return node;
    }
    public Service AddService(Service service)
    {
        Services.Add(service);
        return service;
    }
    public Service AddService(string name, System.Action action)
    {
        Service service = new(name, action);
        Services.Add(service);
        return service;
    }
    public Decorator AddDecorator(Decorator decorator)
    {
        Decorators.Add(decorator);
        return decorator;
    }
    public Decorator AddDecorator(string name, System.Func<bool> func)
    {
        Decorator decorator = new(name, func);
        Decorators.Add(decorator);
        return decorator;
    }
    protected void RunServices()
    {
        for (int i = 0; i < Services.Count; i++)
        {
            Services[i].Evaluate();
        }
    }
    protected bool EvaluateDecorators()
    {
        bool canrun = true;
        for (int i = 0; i < Decorators.Count; i++)
        {
            canrun = Decorators[i].Evaluate();
            if (!canrun)
            {
                break;
            }
        }
        if (canrun != DecoratorsPermitRunning)
        {
            DecoratorsPermitRunning = canrun;
            if (canrun)
            {
                Reset();
            }
        }
        return canrun;
    }
    public virtual void Reset()
    {
        LastState = NodeState.UNKNOWN;
        foreach (Node child in Children)
        {
            child.Reset();
        }
    }
    protected virtual void Abort()
    {
        if (OnExitFunc != null)
        {
            OnExitFunc();
        }
        LastState = NodeState.UNKNOWN;
        foreach (Node child in Children)
        {
            child.Abort();
        }
    }
    protected void Exit()
    {
        if (OnExitFunc != null)
        {
            OnExitFunc();
        }
        foreach (Node child in Children)
        {
            child.Exit();
        }
    }
    protected virtual void OnEnter()
    {
        if (EnterFunc != null)
        {
            LastState = EnterFunc.Invoke();
        }
        else
        {
            LastState = Children.Count > 0 ? NodeState.RUNNING : NodeState.SUCCESS;
        }
    }
    public bool Evaluate()
    {
        bool tickednodes = OnEvaluate();

        // no actions were performed - reset and start over
        if (!tickednodes)
            Reset();

        return tickednodes;
    }
    protected bool OnEvaluate()
    {
        bool tickednodes = false;

        RunServices();

        if (!EvaluateDecorators())
        {
            LastState = NodeState.FAILURE;
            tickednodes = true;
            return tickednodes;
        }

        if (LastState == NodeState.UNKNOWN)
        {
            OnEnter();
            tickednodes = true;
            if (LastState == NodeState.FAILURE)
            {
                return tickednodes;
            }
        }

        if (EvaluateFunc != null)
        {
            LastState = EvaluateFunc();
            tickednodes = true;
            //if we succeeded or failed, we exit
            if (LastState != NodeState.RUNNING)
            {
                Exit();
                return tickednodes;
            }
        }

        if (Children.Count == 0)
        {
            if (EvaluateFunc == null)
            {
                LastState = NodeState.SUCCESS;
                return tickednodes;
            }
        }

        for (int i = 0; i < Children.Count; i++)
        {
            bool childpreviouslyenabled = Children[i].DecoratorsPermitRunning;
            bool childcurrentlyenabled = Children[i].EvaluateDecorators();

            //exit early if we have a child in progress
            if (Children[i].LastState == NodeState.RUNNING)
            {
                tickednodes |= Children[i].Evaluate();
                return tickednodes;
            }

            if (Children[i].LastState != NodeState.UNKNOWN)
            {
                continue;
            }

            tickednodes |= Children[i].Evaluate();

            LastState = Children[i].LastState;

            if (!childpreviouslyenabled && childcurrentlyenabled)
            {
                for (int j = i + 1; j < Children.Count; j++)
                {
                    if (Children[j].LastState == NodeState.RUNNING)
                    {
                        Children[j].Abort();
                    }
                    else
                    {
                        Children[j].Reset();
                    }
                }
            }

            if (Children[i].LastState == NodeState.RUNNING)
            {
                return tickednodes;
            }

            if (Children[i].LastState == NodeState.FAILURE && StopOnChildFailed())
            {
                return tickednodes;
            }
            if (Children[i].LastState == NodeState.RUNNING && StopOnChildNotFailed())
            {
                return tickednodes;
            }
        }

        OnTickedAllChildren();

        return tickednodes;
    }
    protected virtual void OnTickedAllChildren()
    {

    }
    protected virtual bool StopOnChildFailed()
    {
        return false;
    }
    protected virtual bool StopOnChildNotFailed()
    {
        return false;
    }
    public override void GetDebugTextInternal(StringBuilder debugTextBuilder, int indentLevel = 0)
    {
        // apply the indent
        for (int index = 0; index < indentLevel; ++index)
            debugTextBuilder.Append(' ');

        debugTextBuilder.Append($"{Name} [{LastState}]");

        foreach (var service in Services)
        {
            debugTextBuilder.AppendLine();
            debugTextBuilder.Append(service.GetDebugText(indentLevel + 1));
        }

        foreach (var decorator in Decorators)
        {
            debugTextBuilder.AppendLine();
            debugTextBuilder.Append(decorator.GetDebugText(indentLevel + 1));
        }

        foreach (var child in Children)
        {
            debugTextBuilder.AppendLine();
            child.GetDebugTextInternal(debugTextBuilder, indentLevel + 2);
        }
    }
}