using UnityEngine;

public class GameController : MonoBehaviour
{
    [SerializeField] private RouletteController _rouletteController;
    [SerializeField] private UIController _uiController;

    private void Start()
    {
        foreach (Transform button in _rouletteController.GetButtons())
        {
            button.GetComponent<NumberButton>().OnClick += _uiController.UpdateTapText;
            button.GetComponent<NumberButton>().OnBad += _uiController.ShakeText;
        }

        _rouletteController.OnStartSpin += StartGame;
    }

    public void StartGame()
    {
        _rouletteController.OnStartSpin -= StartGame;
        _rouletteController.StartSpin();
        _uiController.UpdateTapText();
    }
}
