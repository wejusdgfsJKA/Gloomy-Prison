using System;
using System.Collections.Generic;

public class BlackBoard
{
    public class Data
    {
        protected object currentValue;
        public object Value
        {
            get
            {
                return currentValue;
            }
            set
            {
                currentValue = value;
                if (OnValueChanged != null)
                {
                    OnValueChanged();
                }
            }
        }

        public event Action OnValueChanged;
        public Data(object _value)
        {
            Value = _value;
        }
    }
    protected Dictionary<string, Data> data = new();
    public void SetData<T>(string _key, T _value)
    {
        if (data.ContainsKey(_key))
        {
            data[_key].Value = _value;
        }
        else
        {
            data.Add(_key, new Data(_value));
        }
    }
    public Data GetData(string _key)
    {
        if (data.ContainsKey(_key))
        {
            return data[_key];
        }
        throw new ArgumentException($"Could not find value for {_key} in Blackboard");
    }
    public BlackBoard()
    {
        data = new();
    }
}