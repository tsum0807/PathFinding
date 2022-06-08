using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

public class PathFinder : MonoBehaviour
{
    private const float SEARCH_SPEED = 0.05f; // Time it takes to search a tile

    [SerializeField] private Tilemap _tileMap;
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

    public static PathFinder Instance { get; private set; }

    private void Awake()
    {
        if (Instance != null && Instance != this)
            Destroy(this);
        else
            Instance = this;
    }

    public void StartPathFinding(Enums.Algorithm algorithm)
    {
        ClearTileMap();
        ClearAllData();
        switch(algorithm)
        {
            case Enums.Algorithm.DFS:
                StartCoroutine(DFS());
                break;

            case Enums.Algorithm.BFS:
                StartCoroutine(BFS());
                break;
        }
    }

    #region algorithms
    private List<Vector3Int> _positionsOpened = new List<Vector3Int>();
    private Queue<TileData> _tilesToSearchQueue = new Queue<TileData>();
    private Stack<TileData> _tilesToSearchStack = new Stack<TileData>();

    IEnumerator DFS()
    {
        float time = 0f;
        TileData curTileData = new TileData(GetStartTilePos());
        // SEARCH
        while(_tileMap.GetTile(curTileData.Position) != _endTile)
        {
            // ORDER: UP -> RIGHT -> DOWN -> LEFT
            if (_tileMap.GetTile(curTileData.Up) != null && _passableTiles.Contains(_tileMap.GetTile(curTileData.Up)) && 
                !_positionsOpened.Contains(curTileData.Up))
            {
                if (_tileMap.GetTile(curTileData.Up) == _endTile)
                {
                    curTileData = new TileData(curTileData.Up, curTileData);
                    break;
                }
                else
                {
                    _tilesToSearchStack.Push(new TileData(curTileData.Up, curTileData));
                    _tileMap.SetTile(curTileData.Up, _toSearchTile);
                    _positionsOpened.Add(curTileData.Up);
                }
            }
            if (_tileMap.GetTile(curTileData.Right) != null && _passableTiles.Contains(_tileMap.GetTile(curTileData.Right)) && 
                !_positionsOpened.Contains(curTileData.Right))
            {
                if (_tileMap.GetTile(curTileData.Right) == _endTile)
                {
                    curTileData = new TileData(curTileData.Right, curTileData);
                    break;
                }
                else
                {
                    _tilesToSearchStack.Push(new TileData(curTileData.Right, curTileData));
                    _tileMap.SetTile(curTileData.Right, _toSearchTile);
                    _positionsOpened.Add(curTileData.Right);
                }
            }
            if (_tileMap.GetTile(curTileData.Down) != null && _passableTiles.Contains(_tileMap.GetTile(curTileData.Down)) && 
                !_positionsOpened.Contains(curTileData.Down))
            {
                if (_tileMap.GetTile(curTileData.Down) == _endTile)
                {
                    curTileData = new TileData(curTileData.Down, curTileData);
                    break;
                }
                else
                {
                    _tilesToSearchStack.Push(new TileData(curTileData.Down, curTileData));
                    _tileMap.SetTile(curTileData.Down, _toSearchTile);
                    _positionsOpened.Add(curTileData.Down);
                }
            }
            if (_tileMap.GetTile(curTileData.Left) != null && _passableTiles.Contains(_tileMap.GetTile(curTileData.Left)) && 
                !_positionsOpened.Contains(curTileData.Left))
            {
                if (_tileMap.GetTile(curTileData.Left) == _endTile)
                {
                    curTileData = new TileData(curTileData.Left, curTileData);
                    break;
                }
                else
                {
                    _tilesToSearchStack.Push(new TileData(curTileData.Left, curTileData));
                    _tileMap.SetTile(curTileData.Left, _toSearchTile);
                    _positionsOpened.Add(curTileData.Left);
                }
            }

            if (_tilesToSearchStack.Count != 0)
            {
                curTileData = _tilesToSearchStack.Pop();
                _tileMap.SetTile(curTileData.Position, _traversedTile);
            }

            time += SEARCH_SPEED;
            yield return new WaitForSeconds(SEARCH_SPEED);
        }

        // GET BEST PATH
        StartCoroutine(TracePath(curTileData));

        yield return null;
    }

    IEnumerator BFS()
    {
        float time = 0f;
        TileData curTileData = new TileData(GetStartTilePos());
        // SEARCH
        while (_tileMap.GetTile(curTileData.Position) != _endTile)
        {
            // ORDER: UP -> RIGHT -> DOWN -> LEFT
            if (_tileMap.GetTile(curTileData.Up) != null && _passableTiles.Contains(_tileMap.GetTile(curTileData.Up)) &&
                !_positionsOpened.Contains(curTileData.Up))
            {
                if (_tileMap.GetTile(curTileData.Up) == _endTile)
                {
                    curTileData = new TileData(curTileData.Up, curTileData);
                    break;
                }
                else
                {
                    _tilesToSearchQueue.Enqueue(new TileData(curTileData.Up, curTileData));
                    _tileMap.SetTile(curTileData.Up, _toSearchTile);
                    _positionsOpened.Add(curTileData.Up);
                }
            }
            if (_tileMap.GetTile(curTileData.Right) != null && _passableTiles.Contains(_tileMap.GetTile(curTileData.Right)) &&
                !_positionsOpened.Contains(curTileData.Right))
            {
                if (_tileMap.GetTile(curTileData.Right) == _endTile)
                {
                    curTileData = new TileData(curTileData.Right, curTileData);
                    break;
                }
                else
                {
                    _tilesToSearchQueue.Enqueue(new TileData(curTileData.Right, curTileData));
                    _tileMap.SetTile(curTileData.Right, _toSearchTile);
                    _positionsOpened.Add(curTileData.Right);
                }
            }
            if (_tileMap.GetTile(curTileData.Down) != null && _passableTiles.Contains(_tileMap.GetTile(curTileData.Down)) &&
                !_positionsOpened.Contains(curTileData.Down))
            {
                if (_tileMap.GetTile(curTileData.Down) == _endTile)
                {
                    curTileData = new TileData(curTileData.Down, curTileData);
                    break;
                }
                else
                {
                    _tilesToSearchQueue.Enqueue(new TileData(curTileData.Down, curTileData));
                    _tileMap.SetTile(curTileData.Down, _toSearchTile);
                    _positionsOpened.Add(curTileData.Down);
                }
            }
            if (_tileMap.GetTile(curTileData.Left) != null && _passableTiles.Contains(_tileMap.GetTile(curTileData.Left)) &&
                !_positionsOpened.Contains(curTileData.Left))
            {
                if (_tileMap.GetTile(curTileData.Left) == _endTile)
                {
                    curTileData = new TileData(curTileData.Left, curTileData);
                    break;
                }
                else
                {
                    _tilesToSearchQueue.Enqueue(new TileData(curTileData.Left, curTileData));
                    _tileMap.SetTile(curTileData.Left, _toSearchTile);
                    _positionsOpened.Add(curTileData.Left);
                }
            }

            if (_tilesToSearchQueue.Count != 0)
            {
                curTileData = _tilesToSearchQueue.Dequeue();
                _tileMap.SetTile(curTileData.Position, _traversedTile);
            }

            time += SEARCH_SPEED;
            yield return new WaitForSeconds(SEARCH_SPEED);
        }

        // GET BEST PATH
        StartCoroutine(TracePath(curTileData));

        yield return null;
    }

    #endregion

    #region helpers

    IEnumerator TracePath(TileData curTileData)
    {
        while (curTileData != null && _tileMap.GetTile(curTileData.Position) != _startTile)
        {
            if (_tileMap.GetTile(curTileData.Position) != _endTile)
            {
                _tileMap.SetTile(curTileData.Position, _pathTile);
            }
            curTileData = curTileData.PreviousTile;
            yield return new WaitForSeconds(SEARCH_SPEED);
        }
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

    private class TileData
    {
        public Vector3Int Position;
        public Enums.TileType TileType;
        public Vector3Int Up;
        public Vector3Int Right;
        public Vector3Int Down;
        public Vector3Int Left;
        public TileData PreviousTile;

        public TileData(Vector3Int pos, TileData prev = null)
        {
            Position = pos;
            PreviousTile = prev;
            Up = Position + new Vector3Int(0, 1, 0);
            Right = Position + new Vector3Int(1, 0, 0);
            Down = Position + new Vector3Int(0, -1, 0);
            Left = Position + new Vector3Int(-1, 0, 0);
        }
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
        _positionsOpened.Clear();
        _tilesToSearchQueue.Clear();
        _tilesToSearchStack.Clear();
    }

    #endregion
}
