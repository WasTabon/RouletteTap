using System;
using DG.Tweening;
using UnityEngine;
using UnityEngine.UI;

public class GameController : MonoBehaviour
{
    [SerializeField] private Camera _mainCamera;
    [SerializeField] private int _tapsCount;
    [SerializeField] private int _taps3Stars;
    [SerializeField] private int _taps2Stars;
    [SerializeField] private int _taps1Star;
    [SerializeField] private RouletteController _rouletteController;
    [SerializeField] private UIController _uiController;
    [SerializeField] private WinPanelController _winPanelController;
    [SerializeField] private SidePanelController _sidePanelController;
    [SerializeField] private AudioSync _audioSync;
    [SerializeField] private Transform _roulette;
    [SerializeField] private AudioClip _showButtonSound;
    [SerializeField] private AudioClip _changeNumberSound;
    [SerializeField] private Image _fade;

    [SerializeField] private GameObject _slowBlack;
    [SerializeField] private GameObject _showBlack;
    [SerializeField] private GameObject _changeBlack;
    
    private Tween _fadeTween;
    
    private AudioSource _audioSource;
    private Vector3 _rouletteSize;

    private NumberButton _buttonWithParticle;
    
    private int _currentGoodTaps;
    private int _currentBadTaps;

    private bool _isWin = true;
    private bool _clickProcessedThisFrame;
    private bool _isPowerupShowButton;
    private bool _isPowerupSlowRoulette;

    private float _rouletteStartSpeed;
    
    private float punchScale = 0.35f;
    private float duration = 0.3f;
    private int vibrato = 10; 
    private float elasticity = 0.8f;

    private void Start()
    {
        if (PlayerPrefs.HasKey("slow"))
        {
            _slowBlack.SetActive(false);
        }
        if (PlayerPrefs.HasKey("show"))
        {
            _showBlack.SetActive(false);
        }
        if (PlayerPrefs.HasKey("change"))
        {
            _changeBlack.SetActive(false);
        }
        
        _audioSource = GetComponent<AudioSource>();
        foreach (Transform button in _rouletteController.GetButtons())
        {
            button.GetComponent<NumberButton>().OnGood += _uiController.AnimateAndUpdateTapText;
            button.GetComponent<NumberButton>().OnBad += _uiController.ShakeText;
            
            button.GetComponent<NumberButton>().OnGood += AddTaps;
            button.GetComponent<NumberButton>().OnBad += AddBadTaps;
            
            button.GetComponent<NumberButton>().OnGood += ShakeRoulette;
            button.GetComponent<NumberButton>().OnBad += ShakeRoulette;
        }
        
        _rouletteStartSpeed = _rouletteController._rotateSpeed;

        _rouletteController.OnStartSpin += StartGame;
    }
    
    private void Update()
    {
        if (!_isWin && !_uiController._isAnim && !_isPowerupShowButton)
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
    }
    
    public void StartGame()
    {
        _rouletteController.OnStartSpin -= StartGame;
        _rouletteSize = _roulette.localScale;
        _rouletteController.StartSpin();
        _uiController.AnimateAndUpdateTapText();
        _uiController.AnimateButtonAppearance();
        _sidePanelController.AnimateButtonAppearance();
        _audioSync.Initialize();
        _isWin = false;
    }
    
    public void HandlePowerupShowButton()
    {
        if (_isPowerupShowButton) return;
        if (_showBlack.activeSelf) return;
        
        _isPowerupShowButton = true;
        Vector3 targetSize = new Vector3(0.03f, 0.03f, 0.03f);
        int neededButton = _uiController.currentNumber;
        int currentButton = neededButton + 1;
        if (currentButton >= _uiController._buttons.Length)
            currentButton = 1;

        Sequence sequence = DOTween.Sequence();

        for (int i = 1; i < _uiController._buttons.Length; i++)
        {
            int buttonIndex = currentButton;

            sequence.AppendCallback(() =>
            {
                _uiController._buttons[buttonIndex]._particle.gameObject.SetActive(true);
                _audioSource.PlayOneShot(_showButtonSound);
            });

            sequence.Append(_uiController._buttons[buttonIndex]._particle
                .DOPunchScale(Vector3.zero, 0f, 8, 0.8f));

            sequence.Append(_uiController._buttons[buttonIndex]._particle
                .DOPunchScale(targetSize, 0.05f, 8, 0.8f));

            if (buttonIndex != neededButton)
            {
                sequence.Append(_uiController._buttons[buttonIndex]._particle
                    .DOPunchScale(Vector3.zero, 0.05f, 8, 0.8f));

                sequence.AppendCallback(() =>
                {
                    _uiController._buttons[buttonIndex]._particle.gameObject.SetActive(false);
                });
            }

            currentButton++;
            if (currentButton >= _uiController._buttons.Length)
                currentButton = 1;
        }
        
        sequence.OnComplete(() =>
        {
            _buttonWithParticle = _uiController._buttons[neededButton];
            _isPowerupShowButton = false;
        });
    }

    public void HandlePowerupSlowRoulette()
    {
        if (_slowBlack.activeSelf) return;
            
        if (!_isPowerupSlowRoulette)
        {
            _isPowerupSlowRoulette = true;
            float currentSpeed = _rouletteController._rotateSpeed;
            float slowSpeed = currentSpeed / 2.5f;

            float currentPitch = _audioSync._audioSource.pitch;
            float slowPitch = 0.5f;

            _rouletteController._rotateSpeed = slowSpeed;

            DOTween.To(() => _rouletteController._rotateSpeed, x => _rouletteController._rotateSpeed = x, slowSpeed, 3f)
                .SetEase(Ease.InOutBounce);

            DOTween.To(() => _audioSync._audioSource.pitch, x => _audioSync._audioSource.pitch = x, slowPitch, 3f)
                .SetEase(Ease.InOutBounce)
                .OnComplete(() =>
                {
                    DOTween.To(() => _rouletteController._rotateSpeed, x => _rouletteController._rotateSpeed = x, _rouletteStartSpeed, 15f);

                    DOTween.To(() => _audioSync._audioSource.pitch, x => _audioSync._audioSource.pitch = x, currentPitch, 15f)
                        .OnComplete(() =>
                        {
                            _isPowerupSlowRoulette = false;
                            StopFadeBlinking();
                        });
                });

            StartFadeBlinking();
        }
    }
    
    private void StartFadeBlinking()
    {
        _fadeTween?.Kill();

        Color startColor = _fade.color;
        startColor.a = 0;

        _fade.color = startColor;

        _fadeTween = _fade.DOFade(0.4f, 0.6f)
            .SetEase(Ease.InOutSine)
            .SetLoops(-1, LoopType.Yoyo);
    }

    private void StopFadeBlinking()
    {
        _fadeTween?.Kill();
        _fadeTween = null;

        Color endColor = _fade.color;
        endColor.a = 0;
        _fade.color = endColor;
    }

    public void HandlePowerupChangeNumber()
    {
        if (_changeBlack.activeSelf) return;
        
        _audioSource.PlayOneShot(_changeNumberSound);
        _uiController.AnimateAndUpdateTapText();
    }

    private void AddTaps()
    {
        _currentGoodTaps++;
        if (_buttonWithParticle != null)
        {
            _buttonWithParticle._particle.DOPunchScale(Vector3.zero, 0.05f, 8, 0.8f)
                .OnComplete((() =>
                {
                    _buttonWithParticle._particle.gameObject.SetActive(false);
                    _buttonWithParticle = null;
                }));
        }
        if (_currentGoodTaps >= _tapsCount)
        {
            _isWin = true;
            _rouletteController.StopSpin();
            _winPanelController.ShowWinPanel(CalculateStars());   
        }
    }

    private void ShakeRoulette()
    {
        _roulette.DOKill();

        _roulette.localScale = _rouletteSize;
        
        _roulette.DOPunchScale(_rouletteSize * punchScale, duration, vibrato, elasticity)
            .SetEase(Ease.OutBack);
    }
    
    private void AddBadTaps()
    {
        _currentBadTaps++;
        if (_buttonWithParticle != null)
        {
            _buttonWithParticle._particle.DOPunchScale(Vector3.zero, 0.05f, 8, 0.8f)
                .OnComplete((() =>
                {
                    _buttonWithParticle._particle.gameObject.SetActive(false);
                    _buttonWithParticle = null;
                }));
        }
    }
    
    private int CalculateStars()
    {
        if (PlayerPrefs.HasKey($"level_stars_{_winPanelController._levelID}"))
        {
            int stars = PlayerPrefs.GetInt($"level_stars_{_winPanelController._levelID}");
            if (stars > CalculateStarsToGet())
                    return stars;
        }
        if (_currentGoodTaps + _currentBadTaps <= _taps3Stars)
            return 3;
        if (_currentGoodTaps + _currentBadTaps <= _taps2Stars)
            return 2;
        if (_currentGoodTaps + _currentBadTaps <= _taps1Star)
            return 1;
        if (_currentGoodTaps + _currentBadTaps > _taps1Star)
            return 1;
    
        return 1;
    }

    private int CalculateStarsToGet()
    {
        if (_currentGoodTaps + _currentBadTaps <= _taps3Stars)
            return 3;
        if (_currentGoodTaps + _currentBadTaps <= _taps2Stars)
            return 2;
        if (_currentGoodTaps + _currentBadTaps <= _taps1Star)
            return 1;
        if (_currentGoodTaps + _currentBadTaps > _taps1Star)
            return 1;
    
        return 1;
    }
}
