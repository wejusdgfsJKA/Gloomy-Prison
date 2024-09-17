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
    protected List<Node> children = new();
    protected List<Service> services = new();
    protected List<Decorator> decorators = new();
    protected System.Func<NodeState> enterFunc;
    protected System.Func<NodeState> evaluateFunc;
    protected System.Action onExitFunc;
    public bool DecoratorsPermitRunning { get; protected set; } = true;
    public Node(string _name = "No Name", System.Func<NodeState> _enter = null, System.Func<NodeState> _evaluate = null, System.Action _onexit = null)
    {
        Name = _name;
        enterFunc = _enter;
        evaluateFunc = _evaluate;
        onExitFunc = _onexit;
    }
    public Node Attach(Node node)
    {
        node.Parent = this;
        children.Add(node);
        return node;
    }
    public Service AddService(Service service)
    {
        services.Add(service);
        return service;
    }
    public Service AddService(string name, System.Action action)
    {
        Service service = new(name, action);
        services.Add(service);
        return service;
    }
    public Decorator AddDecorator(Decorator decorator)
    {
        decorators.Add(decorator);
        return decorator;
    }
    public Decorator AddDecorator(string name, System.Func<bool> func)
    {
        Decorator decorator = new(name, func);
        decorators.Add(decorator);
        return decorator;
    }
    protected void RunServices()
    {
        for (int i = 0; i < services.Count; i++)
        {
            services[i].Evaluate();
        }
    }
    protected bool EvaluateDecorators()
    {
        bool _canrun = true;
        for (int _i = 0; _i < decorators.Count; _i++)
        {
            _canrun = decorators[_i].Evaluate();
            if (!_canrun)
            {
                break;
            }
        }
        if (_canrun != DecoratorsPermitRunning)
        {
            DecoratorsPermitRunning = _canrun;
            if (_canrun)
            {
                Reset();
            }
        }
        return _canrun;
    }
    public virtual void Reset()
    {
        LastState = NodeState.UNKNOWN;
        foreach (Node _child in children)
        {
            _child.Reset();
        }
    }
    protected virtual void Abort()
    {
        if (onExitFunc != null)
        {
            onExitFunc();
        }
        LastState = NodeState.UNKNOWN;
        foreach (Node _child in children)
        {
            _child.Abort();
        }
    }
    protected void Exit()
    {
        if (onExitFunc != null)
        {
            onExitFunc();
        }
        foreach (Node _child in children)
        {
            _child.Exit();
        }
    }
    protected virtual void OnEnter()
    {
        if (enterFunc != null)
        {
            LastState = enterFunc.Invoke();
        }
        else
        {
            LastState = children.Count > 0 ? NodeState.RUNNING : NodeState.SUCCESS;
        }
    }
    public bool Evaluate()
    {
        bool _tickednodes = OnEvaluate();

        // no actions were performed - reset and start over
        if (!_tickednodes)
            Reset();

        return _tickednodes;
    }
    protected bool OnEvaluate()
    {
        bool _tickednodes = false;

        RunServices();

        if (!EvaluateDecorators())
        {
            if (LastState == NodeState.RUNNING)
            {
                Exit();
            }
            LastState = NodeState.FAILURE;
            _tickednodes = true;
            return _tickednodes;
        }

        if (LastState == NodeState.UNKNOWN)
        {
            OnEnter();
            _tickednodes = true;
            if (LastState == NodeState.FAILURE)
            {
                return _tickednodes;
            }
        }

        if (evaluateFunc != null)
        {
            LastState = evaluateFunc();
            _tickednodes = true;
            //if we succeeded or failed, we exit
            if (LastState != NodeState.RUNNING)
            {
                Exit();
                return _tickednodes;
            }
        }

        if (children.Count == 0)
        {
            if (evaluateFunc == null)
            {
                LastState = NodeState.SUCCESS;
                return _tickednodes;
            }
        }

        for (int i = 0; i < children.Count; i++)
        {
            bool childpreviouslyenabled = children[i].DecoratorsPermitRunning;
            bool childcurrentlyenabled = children[i].EvaluateDecorators();

            //exit early if we have a child in progress
            if (children[i].LastState == NodeState.RUNNING)
            {
                _tickednodes |= children[i].Evaluate();
                return _tickednodes;
            }

            if (children[i].LastState != NodeState.UNKNOWN)
            {
                continue;
            }

            _tickednodes |= children[i].Evaluate();

            LastState = children[i].LastState;

            if (!childpreviouslyenabled && childcurrentlyenabled)
            {
                for (int j = i + 1; j < children.Count; j++)
                {
                    if (children[j].LastState == NodeState.RUNNING)
                    {
                        children[j].Abort();
                    }
                    else
                    {
                        children[j].Reset();
                    }
                }
            }

            if (children[i].LastState == NodeState.RUNNING)
            {
                return _tickednodes;
            }

            if (children[i].LastState == NodeState.FAILURE && StopOnChildFailed())
            {
                return _tickednodes;
            }
            if (children[i].LastState == NodeState.RUNNING && StopOnChildNotFailed())
            {
                return _tickednodes;
            }
        }

        OnTickedAllChildren();

        return _tickednodes;
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
    public override void GetDebugTextInternal(StringBuilder _debug, int _indentlevel = 0)
    {
        // apply the indent
        for (int _index = 0; _index < _indentlevel; ++_index)
            _debug.Append(' ');

        _debug.Append($"{Name} [{LastState}]");

        foreach (var _service in services)
        {
            _debug.AppendLine();
            _debug.Append(_service.GetDebugText(_indentlevel + 1));
        }

        foreach (var _decorator in decorators)
        {
            _debug.AppendLine();
            _debug.Append(_decorator.GetDebugText(_indentlevel + 1));
        }

        foreach (var _child in children)
        {
            _debug.AppendLine();
            _child.GetDebugTextInternal(_debug, _indentlevel + 2);
        }
    }
}