using System.Collections.Generic;

public class BlackBoard
{
    protected Dictionary<string, object> data = new();
    public void SetData<T>(string _key, T _value)
    {
        if (data.ContainsKey(_key))
        {
            data[_key] = _value;
        }
        else
        {
            data.Add(_key, _value);
        }
    }
    public T GetData<T>(string _key)
    {
        if (data.ContainsKey(_key))
        {
            return (T)data[_key];
        }
        throw new System.ArgumentException($"Could not find value for {_key} in Blackboard");
    }
    public BlackBoard()
    {
        data = new();
    }
}