using UnityEngine;

public class GameController : MonoBehaviour
{
    [SerializeField] private int _tapsCount;
    [SerializeField] private int _taps3Stars;
    [SerializeField] private int _taps2Stars;
    [SerializeField] private int _taps1Star;
    [SerializeField] private RouletteController _rouletteController;
    [SerializeField] private UIController _uiController;

    private int _currentTaps;

    private void Start()
    {
        foreach (Transform button in _rouletteController.GetButtons())
        {
            button.GetComponent<NumberButton>().OnGood += _uiController.AnimateAndUpdateTapText;
            button.GetComponent<NumberButton>().OnBad += _uiController.ShakeText;
            
            button.GetComponent<NumberButton>().OnGood += AddTaps;
        }

        _rouletteController.OnStartSpin += StartGame;
    }

    public void StartGame()
    {
        _rouletteController.OnStartSpin -= StartGame;
        _rouletteController.StartSpin();
        _uiController.AnimateAndUpdateTapText();
    }

    private void AddTaps()
    {
        _currentTaps++;
        if (_currentTaps >= _tapsCount)
        {
            
        }
    }
}
