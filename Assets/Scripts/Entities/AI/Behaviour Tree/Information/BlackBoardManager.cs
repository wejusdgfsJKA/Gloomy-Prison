using System.Collections.Generic;
using UnityEngine;

public class BlackBoardManager : MonoBehaviour
{
    public static BlackBoardManager instance { get; private set; }
    Dictionary<int, BlackBoard> SharedBlackBoards = new();
    private void Awake()
    {
        if (instance == null)
        {
            instance = this;
        }
    }
    public BlackBoard GetSharedBlackBoard(int _index)
    {
        if (!SharedBlackBoards.ContainsKey(_index))
        {
            SharedBlackBoards[_index] = new BlackBoard();
        }
        return SharedBlackBoards[_index];
    }
}
