using UnityEngine;
using DG.Tweening;
using UnityEngine.UI;

public class SidePanelController : MonoBehaviour
{
    [Header("Panel References")]
    [SerializeField] private RectTransform _sidePanel;
    [SerializeField] private Button _openButton;
    [SerializeField] private Button _closeButton;

    [Header("Buttons Inside Panel")]
    [SerializeField] private RectTransform[] _panelButtons;

    [Header("Canvas Group for Fade")]
    [SerializeField] private CanvasGroup _canvasGroup;

    [Header("Animation Settings")]
    [SerializeField] private float _slideDuration = 0.5f;
    [SerializeField] private Ease _ease = Ease.OutCubic;

    [Header("Sound Effects")]
    [SerializeField] private AudioSource _audioSource;
    [SerializeField] private AudioClip _openSound;
    [SerializeField] private AudioClip _closeSound;

    private Vector2 _hiddenPosition;
    private Vector2 _visiblePosition;
    private bool _isOpen = false;

    private void Start()
    {
        float panelWidth = _sidePanel.rect.width;

        _visiblePosition = _sidePanel.anchoredPosition;
        _hiddenPosition = _visiblePosition + new Vector2(-panelWidth, 0);

        _sidePanel.anchoredPosition = _hiddenPosition;
        _canvasGroup.alpha = 0;
        _canvasGroup.interactable = false;
        _canvasGroup.blocksRaycasts = false;

        foreach (RectTransform button in _panelButtons)
        {
            button.localScale = Vector3.zero;
        }

        _openButton.onClick.AddListener(TogglePanel);
        _closeButton.onClick.AddListener(ClosePanel);
    }

    public void TogglePanel()
    {
        if (_isOpen)
            ClosePanel();
        else
            OpenPanel();
    }
    
    public void OpenPanel()
{
    if (_isOpen) return;
    _isOpen = true;

    _sidePanel.gameObject.SetActive(true);
    _canvasGroup.DOFade(1f, 0.3f);
    _canvasGroup.interactable = true;
    _canvasGroup.blocksRaycasts = true;

    _sidePanel.DOAnchorPos(_visiblePosition, _slideDuration).SetEase(_ease)
        .OnComplete(() =>
        {
            Sequence buttonsSeq = DOTween.Sequence();

            foreach (RectTransform button in _panelButtons)
            {
                Vector3 originalPos = button.localPosition;
                Vector3 jumpPos = originalPos + Vector3.up * 60f;

                button.localScale = Vector3.zero;
                button.localRotation = Quaternion.identity;

                buttonsSeq.AppendCallback(() =>
                {
                    // тряска панели на каждом прыжке
                    _sidePanel.DOShakeAnchorPos(0.1f, 8f, 10, 90f);
                });

                // Появление и прыжок
                buttonsSeq.Append(button.DOScale(new Vector3(1.2f, 0.8f, 1f), 0.15f).SetEase(Ease.OutBack));
                buttonsSeq.Join(button.DOLocalMove(jumpPos, 0.2f).SetEase(Ease.OutQuad));
                buttonsSeq.Join(button.DORotate(new Vector3(0, 0, Random.Range(-15f, 15f)), 0.2f));

                // Возврат и нормализация
                buttonsSeq.Append(button.DOScale(Vector3.one, 0.2f).SetEase(Ease.InOutBack));
                buttonsSeq.Join(button.DOLocalMove(originalPos, 0.25f).SetEase(Ease.InBounce));
                buttonsSeq.Join(button.DORotate(Vector3.zero, 0.25f));
            }
        });

    if (_audioSource && _openSound)
        _audioSource.PlayOneShot(_openSound);
}

public void ClosePanel()
{
    if (!_isOpen) return;
    _isOpen = false;

    Sequence buttonsSeq = DOTween.Sequence();

    foreach (RectTransform button in _panelButtons)
    {
        Vector3 originalPos = button.localPosition;
        Vector3 jumpUpPos = originalPos + Vector3.up * 100f; // Высокий прыжок

        buttonsSeq.AppendCallback(() =>
        {
            // Лёгкая тряска панели перед прыжком
            _sidePanel.DOShakeAnchorPos(0.1f, 8f, 10, 90f);
        });

        // Прыжок вверх с squash и вращением
        buttonsSeq.Append(button.DOScale(new Vector3(0.6f, 1.4f, 1f), 0.15f).SetEase(Ease.OutQuad));
        buttonsSeq.Join(button.DOLocalMove(jumpUpPos, 0.2f).SetEase(Ease.OutQuad));
        buttonsSeq.Join(button.DORotate(new Vector3(0, 0, Random.Range(-20f, 20f)), 0.2f));

        // Падение вниз и исчезновение (влёт в панель)
        buttonsSeq.Append(button.DOScale(Vector3.zero, 0.25f).SetEase(Ease.InBack));
        buttonsSeq.Join(button.DOLocalMove(originalPos, 0.25f).SetEase(Ease.InBounce));
        buttonsSeq.Join(button.DORotate(Vector3.zero, 0.25f));
    }

    buttonsSeq.OnComplete(() =>
    {
        _sidePanel.DOShakeAnchorPos(0.2f, 10f, 15, 90f)
            .OnComplete(() =>
            {
                _canvasGroup.DOFade(0f, 0.3f);
                _canvasGroup.interactable = false;
                _canvasGroup.blocksRaycasts = false;

                _sidePanel.DOAnchorPos(_hiddenPosition, _slideDuration).SetEase(_ease)
                    .OnComplete(() =>
                    {
                        _sidePanel.gameObject.SetActive(false);
                        foreach (RectTransform button in _panelButtons)
                        {
                            button.localScale = Vector3.zero;
                            button.localRotation = Quaternion.identity;
                        }
                    });

                if (_audioSource && _closeSound)
                    _audioSource.PlayOneShot(_closeSound);
            });
    });
}
    
}
