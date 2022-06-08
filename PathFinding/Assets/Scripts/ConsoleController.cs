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

    public void UpdateResults(Enums.Stats stat, int value)
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
        }
    }

}

