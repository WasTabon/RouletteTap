using System.Collections.Generic;
using DG.Tweening;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class WinPanelController : MonoBehaviour
{
    [SerializeField] private int _levelID;
    
    [Header("UI References")]
    [SerializeField] private CanvasGroup _blackBackground;
    [SerializeField] private RectTransform _winPanel;
    [SerializeField] private RectTransform[] _rectTransforms;
    [SerializeField] private RectTransform _congratsText;
    [SerializeField] private RectTransform _continueButton;

    [Header("Animation Settings")]
    [SerializeField] private float _animSpeed = 0.3f;
    [SerializeField] private Ease _ease = Ease.OutBack;

    [Header("Audio")]
    [SerializeField] private AudioClip _panelAppearSound;
    [SerializeField] private AudioClip _starSound;
    [SerializeField] private AudioClip _buttonSound;

    [Header("Particles")]
    [SerializeField] private GameObject _starParticlePrefab;

    private Vector3 _winPanelSize;
    private List<Vector3> _transformSizes;
    private Button _continueBtn;

    private int _starsCount;

    private void Start()
    {
        _transformSizes = new List<Vector3>();
        _winPanelSize = _winPanel.localScale;
        _winPanel.localScale = Vector3.zero;
        _blackBackground.alpha = 0f;
        _blackBackground.gameObject.SetActive(false);

        foreach (RectTransform rectTransform in _rectTransforms)
        {
            _transformSizes.Add(rectTransform.localScale);
            rectTransform.localScale = Vector3.zero;
        }

        _congratsText.localScale = Vector3.zero;
        _continueButton.localScale = Vector3.zero;

        _continueBtn = _continueButton.GetComponent<Button>();
        if (_continueBtn != null)
            _continueBtn.interactable = false;
    }

    public void ShowWinPanel(int starsCount)
    {
        _starsCount = starsCount;

        _blackBackground.gameObject.SetActive(true);
        _winPanel.gameObject.SetActive(true);
        _blackBackground.alpha = 0f;

        int starsShown = 0;
        Sequence sequence = DOTween.Sequence();

        // Fade in background
        sequence.Insert(0, _blackBackground.DOFade(1f, _animSpeed));

        // Panel entrance
        _winPanel.localScale = Vector3.zero;
        _winPanel.rotation = Quaternion.Euler(0, 0, -15);

        if (_panelAppearSound != null)
            AudioSource.PlayClipAtPoint(_panelAppearSound, Camera.main.transform.position);

        sequence.Append(_winPanel.DOScale(1.1f, _animSpeed).SetEase(Ease.OutBack));
        sequence.Join(_winPanel.DORotate(Vector3.zero, _animSpeed).SetEase(Ease.OutCubic));
        sequence.Append(_winPanel.DOScale(1f, 0.1f));

        // Congrats text
        _congratsText.localScale = Vector3.zero;
        sequence.Append(_congratsText.DOScale(1f, _animSpeed).SetEase(Ease.OutBack));

        // Stars and other elements
        for (int i = 0; i < _rectTransforms.Length; i++)
        {
            RectTransform item = _rectTransforms[i];
            item.localScale = Vector3.zero;

            bool isStar = item.CompareTag("Star");
            if (isStar)
            {
                starsShown++;
                if (starsShown > starsCount) continue;
            }

            // Appear
            sequence.Append(item.DOScale(1.2f, _animSpeed).SetEase(_ease));
            sequence.Append(item.DOScale(1f, 0.1f));

            // Punch / bounce effect
            if (isStar)
                sequence.Append(item.DOPunchScale(Vector3.one * 0.2f, 0.3f, 4, 0.5f));

            // Play sound
            if (isStar && _starSound != null)
                sequence.AppendCallback(() => AudioSource.PlayClipAtPoint(_starSound, Camera.main.transform.position));

            // Spawn particles
            if (isStar && _starParticlePrefab != null)
                sequence.AppendCallback(() =>
                    Instantiate(_starParticlePrefab, item.position, Quaternion.identity, item.parent));
        }

        // Continue button
        _continueButton.localScale = Vector3.zero;
        sequence.Append(_continueButton.DOScale(1.1f, _animSpeed).SetEase(Ease.OutBack));
        sequence.Join(_continueButton.DOPunchRotation(new Vector3(0, 0, 10), 0.3f, 3));

        sequence.Append(_continueButton.DOScale(1f, 0.1f));

        // Enable button and play sound
        if (_buttonSound != null)
            sequence.AppendCallback(() => AudioSource.PlayClipAtPoint(_buttonSound, Camera.main.transform.position));

        if (_continueBtn != null)
            sequence.AppendCallback(() => _continueBtn.interactable = true);
    }

    public void LoadLevels()
    {
        PlayerPrefs.SetInt("level", _levelID);
        PlayerPrefs.SetInt("stars", _starsCount);
    }
}
