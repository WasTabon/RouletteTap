using UnityEngine;

public class GameController : MonoBehaviour
{
    [SerializeField] private Camera _mainCamera;
    [SerializeField] private int _tapsCount;
    [SerializeField] private int _taps3Stars;
    [SerializeField] private int _taps2Stars;
    [SerializeField] private int _taps1Star;
    [SerializeField] private RouletteController _rouletteController;
    [SerializeField] private UIController _uiController;

    private int _currentTaps;
    
    private bool _clickProcessedThisFrame;

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
    
    private void Update()
    {
        _clickProcessedThisFrame = false;

        if (Input.GetMouseButtonDown(0) && !_clickProcessedThisFrame)
        {
            HandleClick(Input.mousePosition);
            _clickProcessedThisFrame = true;
        }

        if (Input.touchCount > 0 && Input.GetTouch(0).phase == TouchPhase.Began && !_clickProcessedThisFrame)
        {
            HandleClick(Input.GetTouch(0).position);
            _clickProcessedThisFrame = true;
        }
    }

    private void HandleClick(Vector2 screenPosition)
    {
        Vector2 worldPos = _mainCamera.ScreenToWorldPoint(screenPosition);
        RaycastHit2D hit = Physics2D.Raycast(worldPos, Vector2.zero);

        if (hit.collider != null)
        {
            var button = hit.collider.GetComponent<NumberButton>();
            if (button != null)
            {
                button.OnTap();
            }
        }
        if (hit.collider == null)
            Debug.Log("Missed: " + worldPos);
        else
            Debug.Log("Hit: " + hit.collider.name);
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
