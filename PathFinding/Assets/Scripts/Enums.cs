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

    public enum TileType
    {
        Start = 0,
        End = 1,
        Passable = 2,
        Impassable = 3,
    }

    public enum Stats
    {
        TilesSearched = 0,
        TilesTraversed = 1,
        TilesOnPathFound = 2
    }
}
