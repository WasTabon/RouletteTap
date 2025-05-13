using System;
using UnityEngine;

public class GameController : MonoBehaviour
{
    [SerializeField] private RouletteController _rouletteController;

    private void Start()
    {
        _rouletteController.StartSpin();
    }
}
