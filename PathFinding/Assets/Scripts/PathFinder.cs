using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

public class PathFinder : MonoBehaviour
{
    private const float SEARCH_SPEED_SLOW = 0.1f;
    private const float SEARCH_SPEED_NORMAL = 0.05f;
    private const float SEARCH_SPEED_FAST = 0.02f;

    [SerializeField] private Vector3Int _tileMapSize;
    [SerializeField] private TileBase _startTile;
    [SerializeField] private TileBase _endTile;
    [SerializeField] private TileBase _groundTile;
    [SerializeField] private TileBase _traversedTile;
    [SerializeField] private TileBase _toSearchTile;
    [SerializeField] private TileBase _pathTile;
    [SerializeField] private List<TileBase> _passableTiles;
    [SerializeField] private List<TileBase> _impassableTiles;
    [SerializeField] private List<TileBase> _temporaryTiles;

    private Coroutine _pathFindingCoroutine;
    private Tilemap _tileMap;
    private float _searchSpeed;// Time it takes to search a tile
    private Vector3Int _endPos = Vector3Int.zero;

    public static PathFinder Instance { get; private set; }

    private void Awake()
    {
        if (Instance != null && Instance != this)
            Destroy(this);
        else
            Instance = this;
    }

    public void SetActiveTilemap(Tilemap tilemap)
    {
        _tileMap = tilemap;
    }

    public void StartPathFinding(Enums.Algorithm algorithm)
    {
        GameManager.Instance.EnableButtonsWhilePathFinding(false);
        ClearTileMap();
        ClearAllData();
        ConsoleController.Instance.ClearResults();
        switch (algorithm)
        {
            case Enums.Algorithm.DFS:
                _pathFindingCoroutine = StartCoroutine(DFS());
                break;
            case Enums.Algorithm.BFS:
                _pathFindingCoroutine = StartCoroutine(BFS());
                break;
            case Enums.Algorithm.Random:
                _pathFindingCoroutine = StartCoroutine(RandomPath());
                break;
            case Enums.Algorithm.AStar:
                _pathFindingCoroutine = StartCoroutine(AStar());
                break;
        }
    }

    #region algorithms
    private List<Vector3Int> _positionsSearched = new List<Vector3Int>();
    private Queue<TileData> _tilesToSearchQueue = new Queue<TileData>(); // BFS
    private Stack<TileData> _tilesToSearchStack = new Stack<TileData>(); // DFS
    private List<TileData> _tilesToSearchList = new List<TileData>(); // ASTAR, Random

    IEnumerator DFS()
    {
        TileData curTileData = new TileData(GetStartTilePos(), null, Vector3Int.zero);
        // SEARCH
        while(_tileMap.GetTile(curTileData.Position) != _endTile)
        {
            if (_tilesToSearchStack.Count != 0)
            {
                curTileData = _tilesToSearchStack.Pop();
                TraverseTile(curTileData);
            }

            curTileData = SearchNeighbours(curTileData, null, _tilesToSearchStack);

            yield return new WaitForSeconds(_searchSpeed);
        }

        StartCoroutine(TracePath(curTileData));

        yield return null;
    }

    IEnumerator BFS()
    {
        TileData curTileData = new TileData(GetStartTilePos(), null, Vector3Int.zero);
        // SEARCH
        while (_tileMap.GetTile(curTileData.Position) != _endTile)
        {
            if (_tilesToSearchQueue.Count != 0)
            {
                curTileData = _tilesToSearchQueue.Dequeue();
                TraverseTile(curTileData);
            }

            curTileData = SearchNeighbours(curTileData, _tilesToSearchQueue, null);

            yield return new WaitForSeconds(_searchSpeed);
        }

        StartCoroutine(TracePath(curTileData));

        yield return null;
    }

    IEnumerator RandomPath()
    {
        TileData curTileData = new TileData(GetStartTilePos(), null, Vector3Int.zero);
        // SEARCH
        while (_tileMap.GetTile(curTileData.Position) != _endTile)
        {
            if (_tilesToSearchList.Count != 0)
            {
                int rand = Random.Range(0, _tilesToSearchList.Count);
                curTileData = _tilesToSearchList[rand];
                TraverseTile(curTileData);
                _tilesToSearchList.Remove(curTileData);
            }

            curTileData = SearchNeighbours(curTileData, null, null, _tilesToSearchList);

            yield return new WaitForSeconds(_searchSpeed);
        }

        StartCoroutine(TracePath(curTileData));

        yield return null;
    }

    IEnumerator AStar()
    {
        FindEndPos();
        TileData curTileData = new TileData(GetStartTilePos(), null, _endPos);
        // SEARCH
        while (_tileMap.GetTile(curTileData.Position) != _endTile)
        {
            if (_tilesToSearchList.Count != 0)
                curTileData = _tilesToSearchList[0];
            foreach (TileData tileData in _tilesToSearchList)
            {
                if (tileData.FCost < curTileData.FCost)
                    curTileData = tileData;
            }
            TraverseTile(curTileData);
            if (_tilesToSearchList.Contains(curTileData))
                _tilesToSearchList.Remove(curTileData);

            curTileData = SearchNeighbours(curTileData, null, null, _tilesToSearchList);

            yield return new WaitForSeconds(_searchSpeed);
        }

        // GET BEST PATH
        StartCoroutine(TracePath(curTileData));

        yield return null;
    }

    #endregion

    #region helpers

    private void TraverseTile(TileData tileToTraverse)
    {
        if (_tileMap.GetTile(tileToTraverse.Position) == _startTile)
            return;
        _tileMap.SetTile(tileToTraverse.Position, _traversedTile);
        ConsoleController.Instance.IncrementResult(Enums.Stats.TilesTraversed);
    }

    private void SearchTile(Vector3Int tilePosToSearch)
    {
        _tileMap.SetTile(tilePosToSearch, _toSearchTile);
        _positionsSearched.Add(tilePosToSearch);
        ConsoleController.Instance.IncrementResult(Enums.Stats.TilesSearched);
    }

    private TileData SearchNeighbours(TileData curTileData, Queue<TileData> queue = null, Stack<TileData> stack = null, List<TileData> list = null)
    {
        // ORDER: UP -> RIGHT -> DOWN -> LEFT
        if (_tileMap.GetTile(curTileData.Up) != null && _passableTiles.Contains(_tileMap.GetTile(curTileData.Up)) &&
            !_positionsSearched.Contains(curTileData.Up))
        {
            if (_tileMap.GetTile(curTileData.Up) == _endTile)
            {
                return new TileData(curTileData.Up, curTileData, _endPos);
            }
            else
            {
                if (queue != null)
                    queue.Enqueue(new TileData(curTileData.Up, curTileData, _endPos));
                else if (stack != null)
                    stack.Push(new TileData(curTileData.Up, curTileData, _endPos));
                else if (list != null)
                    list.Add(new TileData(curTileData.Up, curTileData, _endPos));
                SearchTile(curTileData.Up);
            }
        }
        if (_tileMap.GetTile(curTileData.Right) != null && _passableTiles.Contains(_tileMap.GetTile(curTileData.Right)) &&
            !_positionsSearched.Contains(curTileData.Right))
        {
            if (_tileMap.GetTile(curTileData.Right) == _endTile)
            {
                return new TileData(curTileData.Right, curTileData, _endPos);
            }
            else
            {
                if (queue != null)
                    queue.Enqueue(new TileData(curTileData.Right, curTileData, _endPos));
                else if (stack != null)
                    stack.Push(new TileData(curTileData.Right, curTileData, _endPos));
                else if (list != null)
                    list.Add(new TileData(curTileData.Right, curTileData, _endPos));
                SearchTile(curTileData.Right);
            }
        }
        if (_tileMap.GetTile(curTileData.Down) != null && _passableTiles.Contains(_tileMap.GetTile(curTileData.Down)) &&
            !_positionsSearched.Contains(curTileData.Down))
        {
            if (_tileMap.GetTile(curTileData.Down) == _endTile)
            {
                return new TileData(curTileData.Down, curTileData, _endPos);
            }
            else
            {
                if (queue != null)
                    queue.Enqueue(new TileData(curTileData.Down, curTileData, _endPos));
                else if (stack != null)
                    stack.Push(new TileData(curTileData.Down, curTileData, _endPos));
                else if (list != null)
                    list.Add(new TileData(curTileData.Down, curTileData, _endPos));
                SearchTile(curTileData.Down);
            }
        }
        if (_tileMap.GetTile(curTileData.Left) != null && _passableTiles.Contains(_tileMap.GetTile(curTileData.Left)) &&
            !_positionsSearched.Contains(curTileData.Left))
        {
            if (_tileMap.GetTile(curTileData.Left) == _endTile)
            {
                return new TileData(curTileData.Left, curTileData, _endPos);
            }
            else
            {
                if (queue != null)
                    queue.Enqueue(new TileData(curTileData.Left, curTileData, _endPos));
                else if (stack != null)
                    stack.Push(new TileData(curTileData.Left, curTileData, _endPos));
                else if (list != null)
                    list.Add(new TileData(curTileData.Left, curTileData, _endPos));
                SearchTile(curTileData.Left);
            }
        }
        return curTileData;
    }

    private void FindEndPos()
    {
        Vector3Int curTile = new Vector3Int(0, 0, 0);
        while (_tileMap.GetTile(curTile) != null)
        {
            if (_tileMap.GetTile(curTile) == _endTile)
            {
                _endPos = curTile;
                return;
            }
            if (_tileMap.GetTile(curTile + new Vector3Int(1, 0, 0)) == null)
            {
                curTile.x = 0;
                curTile.y++;
            }
            else
            {
                curTile.x++;
            }
        }
    }

    IEnumerator TracePath(TileData curTileData)
    {
        while (curTileData != null && _tileMap.GetTile(curTileData.Position) != _startTile)
        {
            if (_tileMap.GetTile(curTileData.Position) != _endTile)
            {
                _tileMap.SetTile(curTileData.Position, _pathTile);
            }
            ConsoleController.Instance.IncrementResult(Enums.Stats.TilesOnPathFound);
            curTileData = curTileData.PreviousTile;
            yield return new WaitForSeconds(_searchSpeed);
        }
        GameManager.Instance.EnableButtonsWhilePathFinding(true);
        yield return null;
    }

    private Vector3Int GetStartTilePos()
    {
        Vector3Int curTile = new Vector3Int(0, 0, 0);
        while (_tileMap.GetTile(curTile) != _startTile)
        {
            if (_tileMap.GetTile(curTile + new Vector3Int(1, 0, 0)) == null)
            {
                curTile.x = 0;
                curTile.y++;
            }
            else
            {
                curTile.x++;
            }
        }
        return curTile;
    }

    public void ClearTileMap()
    {
        Vector3Int curTile = new Vector3Int(0, 0, 0);
        while (_tileMap.GetTile(curTile) != null)
        {
            if(_temporaryTiles.Contains(_tileMap.GetTile(curTile)))
            {
                _tileMap.SetTile(curTile, _groundTile);
            }
            if (_tileMap.GetTile(curTile + new Vector3Int(1, 0, 0)) == null)
            {
                curTile.x = 0;
                curTile.y++;
            }
            else
            {
                curTile.x++;
            }
        }
    }

    public void ClearAllData()
    {
        _positionsSearched.Clear();
        _tilesToSearchQueue.Clear();
        _tilesToSearchStack.Clear();
        _tilesToSearchList.Clear();
    }

    public void StopPathFindingCoroutine()
    {
        StopCoroutine(_pathFindingCoroutine);
    }

    public void SetSearchSpeed(Enums.Speed speed)
    {
        switch(speed)
        {
            case Enums.Speed.Slow:
                _searchSpeed = SEARCH_SPEED_SLOW;
                break;
            case Enums.Speed.Normal:
                _searchSpeed = SEARCH_SPEED_NORMAL;
                break;
            case Enums.Speed.Fast:
                _searchSpeed = SEARCH_SPEED_FAST;
                break;
        }
    }

    #endregion

    private class TileData
    {
        public Vector3Int Position;
        public Enums.TileType TileType;
        public Vector3Int Up;
        public Vector3Int Right;
        public Vector3Int Down;
        public Vector3Int Left;
        public TileData PreviousTile;
        // FOR A STAR
        public int DistToStart;
        public int DistToEnd;
        public int FCost => DistToEnd + DistToStart;


        public TileData(Vector3Int pos, TileData prev, Vector3Int endPos)
        {
            Position = pos;
            PreviousTile = prev;

            Up = Position + new Vector3Int(0, 1, 0);
            Right = Position + new Vector3Int(1, 0, 0);
            Down = Position + new Vector3Int(0, -1, 0);
            Left = Position + new Vector3Int(-1, 0, 0);

            if (prev == null)
                DistToStart = 0;
            else
                DistToStart = PreviousTile.DistToStart + 1;

            if (endPos != Vector3Int.zero)
                DistToEnd = Mathf.Abs(endPos.x - Position.x) + Mathf.Abs(endPos.y - Position.y);
        }
    }
}
