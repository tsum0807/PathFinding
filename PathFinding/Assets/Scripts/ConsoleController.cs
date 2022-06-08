using System.Collections;
using System.Collections.Generic;
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

    //[SerializeField] private TMPro

    //public void UpdateResults(Enums.Stats stat, int value)
    //{
    //    switch(stat)
    //    {
    //        case Enums.Stats.TilesSearched:

    //    }
    //}

}

