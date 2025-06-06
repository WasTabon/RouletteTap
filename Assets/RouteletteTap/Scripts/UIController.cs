using DG.Tweening;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
using Random = UnityEngine.Random;

public class UIController : MonoBehaviour
{
    [SerializeField] private GameObject _menuButton;
    [SerializeField] private TextMeshProUGUI _buttonToTapText;
    [SerializeField] public NumberButton[] _buttons;
    [SerializeField] private int _buttonsCount;

    [SerializeField] private AudioSource _audioSource;
    [SerializeField] private AudioClip _clip;
    
    public int currentNumber;
    
    public bool _isAnim;

    private void Awake()
    {
        _buttonToTapText.text = "";
        
        RectTransform openButtonRect = _menuButton.GetComponent<RectTransform>();
        openButtonRect.localScale = Vector3.zero;
    }

    public void AnimateButtonAppearance()
    {
        RectTransform openButtonRect = _menuButton.GetComponent<RectTransform>();
        openButtonRect.localScale = Vector3.zero;
        openButtonRect.localRotation = Quaternion.Euler(0, 0, 30f);

        Sequence seq = DOTween.Sequence();

        seq.Append(openButtonRect.DOScale(1.1f, 0.6f).SetEase(Ease.OutElastic));
        seq.Join(openButtonRect.DORotate(Vector3.zero, 0.6f).SetEase(Ease.OutBack));

        seq.Append(openButtonRect.DOScale(1f, 0.3f).SetEase(Ease.InOutSine));
        seq.AppendInterval(0.2f);
        
        if (_audioSource && _clip)
            _audioSource.PlayOneShot(_clip);
    }
    
    #region Taps

    public void AnimateAndUpdateTapText()
    {
        RectTransform rect = _buttonToTapText.rectTransform;

        rect.DOAnchorPosX(-500, 0.3f).SetEase(Ease.InBack)
            .OnStart((() =>
            {
                _isAnim = true;
            }))
            .OnComplete(() =>
            {
                UpdateTapText();
                rect.anchoredPosition = new Vector2(500, rect.anchoredPosition.y);
                rect.DOAnchorPosX(0, 0.3f).SetEase(Ease.OutBack)
                    .OnComplete((() =>
                    {
                        _isAnim = false;
                    }));
            });
    }
    
    private void UpdateTapText()
    {
        foreach (var button in _buttons)
        {
            if (button != null)
                button.isClickable = false;
        }
        
        int result = Random.Range(1, _buttonsCount + 1);
        currentNumber = result;
        string displayText = GenerateExpression(result);
        _buttonToTapText.text = displayText;
        _buttons[result].isClickable = true;
    }

    public void ShakeText()
    {
        RectTransform rectTransform = _buttonToTapText.rectTransform;

        rectTransform.DOShakeAnchorPos(
                duration: 0.5f,
                strength: new Vector2(50f, 0f),
                vibrato: 50,
                randomness: 90f,
                snapping: false,
                fadeOut: true
            )
            .OnStart((() =>
            {
                _isAnim = true;
            }))
            .OnComplete((() =>
            {
                AnimateAndUpdateTapText();
            }));
    }

    private string GenerateExpression(int result)
    {
        if (Random.value < 0.5f)
            return result.ToString();

        string expression = result.ToString();

        float chance = Random.value;

        if (chance < 0.25f)
        {
            int a = Random.Range(1, result);
            int b = result - a;
            expression = $"{a} + {b}";
        }
        else if (chance < 0.5f)
        {
            int b = Random.Range(1, _buttonsCount - result + 1);
            int a = result + b;
            if (a <= _buttonsCount)
                expression = $"{a} - {b}";
        }
        else if (chance < 0.75f)
        {
            for (int i = 2; i <= result; i++)
            {
                if (result % i == 0)
                {
                    int b = i;
                    int a = result / i;
                    expression = $"{a} * {b}";
                    break;
                }
            }
        }
        else
        {
            int maxB = _buttonsCount / result;
            if (maxB >= 1)
            {
                int b = Random.Range(1, maxB + 1);
                int a = result * b;
                expression = $"{a} / {b}";
            }
        }

        return expression;
    }

    #endregion

    public void LoadMenu()
    {
        SceneManager.LoadScene("Levels");
    }
}