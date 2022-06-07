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
    [SerializeField] private TileBase _groundTile;
    [SerializeField] private TileBase _wallTile;


    public void StartPathFinding(Enums.Algorithm algorithm)
    {
        StartCoroutine(Test());
    }

    IEnumerator DFS()
    {


        yield return null;
    }

    IEnumerator Test()
    {
        Vector3Int curCell = new Vector3Int(0, 0, 0);
        while (_tileMap.GetTile(curCell) != null)
        {
            _tileMap.SetTile(curCell, _startTile);
            if (_tileMap.GetTile(curCell + new Vector3Int(1, 0, 0)) == null)
            {
                curCell.x = 0;
                curCell.y++;
            }
            else
            {
                curCell.x++;
            }
            yield return new WaitForSeconds(0.1f);
        }
        yield return null;
    }
}
