using DG.Tweening;
using TMPro;
using UnityEngine;

public class UIController : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI _buttonToTapText;
    [SerializeField] private NumberButton[] _buttons;
    [SerializeField] private int _buttonsCount;

    #region Taps

    public void AnimateAndUpdateTapText()
    {
        RectTransform rect = _buttonToTapText.rectTransform;

        rect.DOAnchorPosX(-500, 0.3f).SetEase(Ease.InBack)
            .OnComplete(() =>
            {
                UpdateTapText();
                rect.anchoredPosition = new Vector2(500, rect.anchoredPosition.y);
                rect.DOAnchorPosX(0, 0.3f).SetEase(Ease.OutBack);
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
        string displayText = GenerateExpression(result);
        _buttonToTapText.text = displayText;
        _buttons[result].isClickable = true;
    }

    public void ShakeText()
    {
        RectTransform rectTransform = _buttonToTapText.rectTransform;

        rectTransform.DOShakeAnchorPos(
                duration: 0.5f,
                strength: new Vector2(30f, 0f),
                vibrato: 50,
                randomness: 90f,
                snapping: false,
                fadeOut: true
            )
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

    public void ShowWinPanel(int starsCount)
    {
        
    }
}