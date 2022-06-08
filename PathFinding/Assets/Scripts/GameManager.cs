using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;
using UnityEngine.UI;

public class GameManager : MonoBehaviour
{
    [SerializeField] private List<Tilemap> _tilemaps;

    public static GameManager Instance { get; private set; }

    private Enums.Algorithm _selectedAlgorithm = Enums.Algorithm.DFS;

    private void Awake()
    {
        if (Instance != null && Instance != this)
            Destroy(this);
        else
            Instance = this;
    }

    private void Start()
    {
        OnSpeedOptionButtonPressed(1);
        OnTileMapButtonPressed(0);
        OnAlgorithmButtonPressed(0);
    }

    private void SelectTilemap(int tilemapID)
    {
        foreach(Tilemap tilemap in _tilemaps)
        {
            tilemap.gameObject.SetActive(false);
        }
        _tilemaps[tilemapID].gameObject.SetActive(true);
    }

    #region Buttons

    [SerializeField] List<Button> _algoButtons;
    [SerializeField] List<Button> _tileSelectionButtons;
    [SerializeField] List<Button> _speedButtons;
    [SerializeField] Button _startButton;
    [SerializeField] List<GameObject> _buttonBlockers;

    public void OnAlgorithmButtonPressed(int buttonIndex)
    {
        _selectedAlgorithm = (Enums.Algorithm)buttonIndex;
        foreach(Button button in _algoButtons)
        {
            button.interactable = true;
        }
        _algoButtons[buttonIndex].interactable = false;
    }

    public void OnTileMapButtonPressed(int tilemapID)
    {
        foreach (Button button in _tileSelectionButtons)
        {
            button.interactable = true;
        }
        _tileSelectionButtons[tilemapID].interactable = false;

        switch (tilemapID)
        {
            case 0:
                ConsoleController.Instance.SetResult(Enums.Stats.BestPath, 26);
                break;
            case 1:
                ConsoleController.Instance.SetResult(Enums.Stats.BestPath, 16);
                break;
            case 2:
                ConsoleController.Instance.SetResult(Enums.Stats.BestPath, 30);
                break;
            case 3:
                ConsoleController.Instance.SetResult(Enums.Stats.BestPath, 40);
                break;
            case 4:
                ConsoleController.Instance.SetResult(Enums.Stats.BestPath, 46);
                break;
            case 5:
                ConsoleController.Instance.SetResult(Enums.Stats.BestPath, 3);
                break;
            case 6:
                ConsoleController.Instance.SetResult(Enums.Stats.BestPath, 128);
                break;
            case 7:
                ConsoleController.Instance.SetResult(Enums.Stats.BestPath, 120);
                break;
        }
        PathFinder.Instance.SetActiveTilemap(_tilemaps[tilemapID]);
        SelectTilemap(tilemapID);
    }

    public void OnStartButtonPressed()
    {
        PathFinder.Instance.StartPathFinding(_selectedAlgorithm);
    }

    public void OnClearButtonPressed()
    {
        PathFinder.Instance.ClearTileMap();
        PathFinder.Instance.ClearAllData();
        PathFinder.Instance.StopPathFindingCoroutine();
        ConsoleController.Instance.ClearResults();
        EnableButtonsWhilePathFinding(true);
    }

    public void OnSpeedOptionButtonPressed(int speed)
    {
        foreach (Button button in _speedButtons)
        {
            button.interactable = true;
        }
        _speedButtons[speed].interactable = false;
        PathFinder.Instance.SetSearchSpeed((Enums.Speed)speed);
    }

    public void EnableButtonsWhilePathFinding(bool flag = true)
    {
        foreach (GameObject blocker in _buttonBlockers)
        {
            blocker.SetActive(!flag);
        }
    }

    #endregion
}
