using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class GameManager : MonoBehaviour
{
    public static GameManager Instance { get; private set; }

    private Enums.Algorithm _selectedAlgorithm = Enums.Algorithm.DFS;

    private void Awake()
    {
        if (Instance != null && Instance != this)
            Destroy(this);
        else
            Instance = this;
    }

    #region Buttons

    [SerializeField] List<Button> _algoButtons;

    public void OnAlgorithmButtonPressed(int buttonIndex)
    {
        _selectedAlgorithm = (Enums.Algorithm)buttonIndex;
        foreach(Button button in _algoButtons)
        {
            button.interactable = true;
        }
        _algoButtons[buttonIndex].interactable = false;
    }

    public void OnStartButtonPressed()
    {
        PathFinder.Instance.StartPathFinding(_selectedAlgorithm);
    }

    #endregion
}
