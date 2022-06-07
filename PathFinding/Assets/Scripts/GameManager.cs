using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class GameManager : MonoBehaviour
{
    public static GameManager Instance;

    private Enums.Algorithm _selectedAlgorithm;

    private void Awake()
    {
        if (Instance == null)
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

    }

    #endregion
}
