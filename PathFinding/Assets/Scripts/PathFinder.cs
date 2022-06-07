using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

public class PathFinder : MonoBehaviour
{
    [SerializeField] private Tilemap _tileMap;
    [SerializeField] private Vector3Int _tileMapSize;
    [SerializeField] private TileBase _startTile;
    [SerializeField] private TileBase _endTile;
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

    IEnumerator DFS()
    {
        Vector3Int curTile = GetStartTilePos();
        print(curTile);

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
