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
    public BlackBoard GetSharedBlackBoard(int index)
    {
        if (!SharedBlackBoards.ContainsKey(index))
        {
            SharedBlackBoards[index] = new BlackBoard();
        }
        return SharedBlackBoards[index];
    }
}
