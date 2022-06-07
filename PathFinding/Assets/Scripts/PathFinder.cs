using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

public class PathFinder : MonoBehaviour
{
    private const float SEARCH_SPEED = 0.05f; // Time it takes to search a tile
    private const float MAX_SEARCH_TIME = 5f;

    [SerializeField] private Tilemap _tileMap;
    [SerializeField] private Vector3Int _tileMapSize;
    [SerializeField] private TileBase _startTile;
    [SerializeField] private TileBase _endTile;
    [SerializeField] private TileBase _traversedTile;
    [SerializeField] private List<TileBase> _passableTiles;
    [SerializeField] private List<TileBase> _impassableTiles;

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
        StartCoroutine(DFS());
    }

    #region algorithms
    private List<TileData> _traversedTiles = new List<TileData>();

    IEnumerator DFS()
    {
        float time = 0f;
        Vector3Int curTile = GetStartTilePos();
        // SEARCH
        while(_tileMap.GetTile(curTile) != _endTile)
        {
            // ORDER: UP -> RIGHT -> DOWN -> LEFT
            TileData curTileData = new TileData(curTile);
            if (_passableTiles.Contains(_tileMap.GetTile(curTileData.Up)))
            {
                if (_tileMap.GetTile(curTile) != _startTile)
                    _tileMap.SetTile(curTile, _traversedTile);
                _traversedTiles.Add(curTileData);
                curTile = curTileData.Up;
            }
            else if (_passableTiles.Contains(_tileMap.GetTile(curTileData.Right)))
            {
                if (_tileMap.GetTile(curTile) != _startTile)
                    _tileMap.SetTile(curTile, _traversedTile);
                _traversedTiles.Add(curTileData);
                curTile = curTileData.Right;
            }
            else if (_passableTiles.Contains(_tileMap.GetTile(curTileData.Down)))
            {
                if (_tileMap.GetTile(curTile) != _startTile)
                    _tileMap.SetTile(curTile, _traversedTile);
                _traversedTiles.Add(curTileData);
                curTile = curTileData.Down;
            }
            else if (_passableTiles.Contains(_tileMap.GetTile(curTileData.Left)))
            {
                if (_tileMap.GetTile(curTile) != _startTile)
                    _tileMap.SetTile(curTile, _traversedTile);
                _traversedTiles.Add(curTileData);
                curTile = curTileData.Left;
            }

            time += SEARCH_SPEED;
            if (time > MAX_SEARCH_TIME)
                break;
            yield return new WaitForSeconds(SEARCH_SPEED);
        }
        // GET BEST PATH
        yield return null;
    }

    #endregion

    #region helpers

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

        public TileData(Vector3Int pos)
        {
            Position = pos;
            Up = Position + new Vector3Int(0, 1, 0);
            Right = Position + new Vector3Int(1, 0, 0);
            Down = Position + new Vector3Int(0, -1, 0);
            Left = Position + new Vector3Int(-1, 0, 0);
        }
    }

    IEnumerator Test()
    {
        Vector3Int curTile = new Vector3Int(0, 0, 0);
        while (_tileMap.GetTile(curTile) != null)
        {
            _tileMap.SetTile(curTile, _startTile);
            if (_tileMap.GetTile(curTile + new Vector3Int(1, 0, 0)) == null)
            {
                curTile.x = 0;
                curTile.y++;
            }
            else
            {
                curTile.x++;
            }
            yield return new WaitForSeconds(0.1f);
        }
        yield return null;
    }

    #endregion
}
