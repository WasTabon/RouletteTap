using UnityEngine;
using DG.Tweening;
using UnityEngine.UI;
using Random = UnityEngine.Random;

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
    [SerializeField] private AudioClip _showButtonSound;
    [SerializeField] private AudioClip _slideSound;
    [SerializeField] private AudioClip[] _hideButtonSounds;
    [SerializeField] private AudioClip _closeSound;

    private Vector2 _hiddenPosition;
    private Vector2 _visiblePosition;
    private bool _isOpen = false;
    private bool _isAnimating = false;

    private void Awake()
    {
        _openButton.GetComponent<RectTransform>().localScale = Vector3.zero;
    }

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
        if (_isAnimating) return;
        if (_isOpen)
            ClosePanel();
        else
            OpenPanel();
    }

    public void OpenPanel()
    {
        if (_isOpen || _isAnimating) return;
        _isAnimating = true;
        _isOpen = true;

        _openButton.transform.DOKill();
        _openButton.transform.DOPunchScale(Vector3.one * 0.4f, 0.3f, 8, 0.8f)
            .SetEase(Ease.OutBack);
        
        _sidePanel.gameObject.SetActive(true);
        _canvasGroup.DOFade(1f, 0.3f);
        _canvasGroup.interactable = true;
        _canvasGroup.blocksRaycasts = true;

        _sidePanel.DOAnchorPos(_visiblePosition, _slideDuration).SetEase(_ease)
            .OnStart((() =>
            {
                _audioSource.PlayOneShot(_slideSound);
            }))
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
                        _sidePanel.DOShakeAnchorPos(0.1f, 8f, 10, 90f);
                    });

                    buttonsSeq.Append(button.DOScale(new Vector3(0.8f, 1.2f, 1f), 0.15f).SetEase(Ease.OutBack)
                        .OnStart((() =>
                        {
                            int random = Random.Range(0, _hideButtonSounds.Length);
                            _audioSource.PlayOneShot(_hideButtonSounds[random]);
                        })));
                    buttonsSeq.Join(button.DOLocalMove(jumpPos, 0.2f).SetEase(Ease.OutQuad));
                    buttonsSeq.Join(button.DORotate(new Vector3(0, 0, Random.Range(-15f, 15f)), 0.2f));

                    buttonsSeq.Append(button.DOScale(Vector3.one, 0.2f).SetEase(Ease.InOutBack));
                    buttonsSeq.Join(button.DOLocalMove(originalPos, 0.25f).SetEase(Ease.InBounce));
                    buttonsSeq.Join(button.DORotate(Vector3.zero, 0.25f));
                }

                buttonsSeq.OnComplete(() =>
                {
                    _isAnimating = false;
                });
            });

        if (_audioSource && _openSound)
            _audioSource.PlayOneShot(_openSound);
    }

    public void ClosePanel()
    {
        if (!_isOpen || _isAnimating) return;
        _isAnimating = true;
        _isOpen = false;

        Sequence buttonsSeq = DOTween.Sequence();

        foreach (RectTransform button in _panelButtons)
        {
            Vector3 originalPos = button.localPosition;
            Vector3 jumpUpPos = originalPos + Vector3.up * 100f;

            buttonsSeq.AppendCallback(() =>
            {
                _sidePanel.DOShakeAnchorPos(0.1f, 8f, 10, 90f);
            });

            buttonsSeq.Append(button.DOScale(new Vector3(1.4f, 0.6f, 1f), 0.15f).SetEase(Ease.OutQuad)
                .OnStart((() =>
                {
                    int random = Random.Range(0, _hideButtonSounds.Length);
                    _audioSource.PlayOneShot(_hideButtonSounds[random]);
                })));
            buttonsSeq.Join(button.DOLocalMove(jumpUpPos, 0.2f).SetEase(Ease.OutQuad));
            buttonsSeq.Join(button.DORotate(new Vector3(0, 0, Random.Range(-20f, 20f)), 0.2f));

            buttonsSeq.Append(button.DOScale(Vector3.zero, 0.25f).SetEase(Ease.InBack));
            buttonsSeq.Join(button.DOLocalMove(originalPos, 0.25f).SetEase(Ease.InBounce));
            buttonsSeq.Join(button.DORotate(Vector3.zero, 0.25f));
        }

        buttonsSeq.OnComplete(() =>
        {
            _sidePanel.DOShakeAnchorPos(0.2f, 10f, 15, 90f)
                .OnComplete(() =>
                {
                    _audioSource.PlayOneShot(_slideSound);
                    _canvasGroup.DOFade(0f, 0.3f);
                    _canvasGroup.interactable = false;
                    _canvasGroup.blocksRaycasts = false;

                    _sidePanel.DOAnchorPos(_hiddenPosition, _slideDuration).SetEase(_ease)
                        .OnStart((() =>
                        {
                            _openButton.transform.DOKill();
                            _openButton.transform.DOPunchScale(Vector3.one * 0.4f, 0.3f, 8, 0.8f)
                                .SetEase(Ease.OutBack);
                        }))
                        .OnComplete(() =>
                        {
                            _sidePanel.gameObject.SetActive(false);
                            foreach (RectTransform button in _panelButtons)
                            {
                                button.localScale = Vector3.zero;
                                button.localRotation = Quaternion.identity;
                            }
                            _isAnimating = false;
                        });

                    if (_audioSource && _closeSound)
                        _audioSource.PlayOneShot(_closeSound);
                });
        });
    }
    
    public void AnimateButtonAppearance()
    {
        RectTransform openButtonRect = _openButton.GetComponent<RectTransform>();
        openButtonRect.localScale = Vector3.zero;
        openButtonRect.localRotation = Quaternion.Euler(0, 0, 30f);

        Sequence seq = DOTween.Sequence();

        seq.Append(openButtonRect.DOScale(1.1f, 0.6f).SetEase(Ease.OutElastic));
        seq.Join(openButtonRect.DORotate(Vector3.zero, 0.6f).SetEase(Ease.OutBack));

        seq.Append(openButtonRect.DOScale(1f, 0.3f).SetEase(Ease.InOutSine));
        seq.AppendInterval(0.2f);

        // (опционально) звук при появлении
        if (_audioSource && _showButtonSound)
            _audioSource.PlayOneShot(_showButtonSound);
    }
}
