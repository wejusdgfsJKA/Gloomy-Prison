using System.Collections.Generic;
using UnityEngine;

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
}
public class BlackBoardManager : MonoBehaviour
{
    public static BlackBoardManager instance { get; private set; }
    Dictionary<MonoBehaviour, BlackBoard> IndividualBlackBoards = new();
    Dictionary<int, BlackBoard> SharedBlackBoards = new();
    private void Awake()
    {
        if (instance == null)
        {
            instance = this;
        }
    }
    public BlackBoard GetIndividualBlackBoard(MonoBehaviour monoBehaviour)
    {
        if (!IndividualBlackBoards.ContainsKey(monoBehaviour))
        {
            IndividualBlackBoards[monoBehaviour] = new BlackBoard();
        }
        return IndividualBlackBoards[monoBehaviour];
    }

    public BlackBoard GetSharedBlackBoard(int index)
    {
        if (!SharedBlackBoards.ContainsKey(index))
        {
            SharedBlackBoards[index] = new BlackBoard();
        }
        return SharedBlackBoards[index];
    }
}
