using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class ConsoleController : MonoBehaviour
{
    public static ConsoleController Instance { get; private set; }
    private void Awake()
    {
        if (Instance != null && Instance != this)
            Destroy(this);
        else
            Instance = this;
    }

    [SerializeField] private TextMeshProUGUI _tilesSearchedText;
    [SerializeField] private TextMeshProUGUI _tilesTraversedText;
    [SerializeField] private TextMeshProUGUI _pathFoundDistanceText;
    [SerializeField] private TextMeshProUGUI _bestPathText;

    public void SetResult(Enums.Stats stat, int value)
    {
        switch (stat)
        {
            case Enums.Stats.TilesSearched:
                _tilesSearchedText.text = value.ToString();
                break;
            case Enums.Stats.TilesTraversed:
                _tilesTraversedText.text = value.ToString();
                break;
            case Enums.Stats.TilesOnPathFound:
                _pathFoundDistanceText.text = value.ToString();
                break;
            case Enums.Stats.BestPath:
                _bestPathText.text = value.ToString();
                break;
        }
    }

    public void IncrementResult(Enums.Stats stat, int value = 1)
    {
        switch (stat)
        {
            case Enums.Stats.TilesSearched:
                _tilesSearchedText.text = (int.Parse(_tilesSearchedText.text) + value).ToString();
                break;
            case Enums.Stats.TilesTraversed:
                _tilesTraversedText.text = (int.Parse(_tilesTraversedText.text) + value).ToString();
                break;
            case Enums.Stats.TilesOnPathFound:
                _pathFoundDistanceText.text = (int.Parse(_pathFoundDistanceText.text) + value).ToString();
                break;
            case Enums.Stats.BestPath:
                _bestPathText.text = (int.Parse(_bestPathText.text) + value).ToString();
                break;
        }
    }

    public void ClearResults()
    {
        _tilesSearchedText.text = "0";
        _tilesTraversedText.text = "0";
        _pathFoundDistanceText.text = "0";
        _bestPathText.text = "0";
    }
}

