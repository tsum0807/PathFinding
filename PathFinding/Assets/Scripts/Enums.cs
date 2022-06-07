using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Enums
{
    [System.Serializable]
    public enum Algorithm
    {
        DFS = 0,
        BFS = 1,
        Djik = 2,
        AStar = 3,
    }

    public enum Tile
    {
        Start = 0,
        End = 1,
        Road = 2,
        Wall = 3,
    }
}
