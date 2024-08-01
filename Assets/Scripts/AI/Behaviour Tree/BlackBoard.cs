using System.Collections.Generic;

public class BlackBoard
{
    Dictionary<string, object> data = new();
    public void SetData<T>(string key, T value)
    {
        if (data.ContainsKey(key))
        {
            data[key] = value;
        }
        else
        {
            data.Add(key, value);
        }
    }
    public T GetData<T>(string key)
    {
        if (data.ContainsKey(key))
        {
            return (T)data[key];
        }
        throw new System.ArgumentException($"Could not find value for {key} in Blackboard");
    }
    public BlackBoard()
    {
        data = new();
    }
}