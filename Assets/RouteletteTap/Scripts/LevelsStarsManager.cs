using System.Collections.Generic;
using System.Linq;
using DG.Tweening;
using UnityEngine;
using UnityEngine.UI;

public class LevelsStarsManager : MonoBehaviour
{
    [SerializeField] private RectTransform[] _levels;
    [SerializeField] private RectTransform[] _levelPath;
    [SerializeField] private AudioClip[] _starAppearSounds;
    [SerializeField] private AudioClip[] _pathAppearSounds;
    [SerializeField] private AudioClip _lockAppearSound;

    private AudioSource _audioSource;
    
    private int _level;
    private int _stars;

    private int _modifier = 1;

    private void Start()
    {
        _audioSource = GetComponent<AudioSource>();
        
        LoadLevels();

        if (PlayerPrefs.HasKey("level"))
        {
            _level = PlayerPrefs.GetInt("level");
            _stars = PlayerPrefs.GetInt("stars");
            
            if (_level >= 4)
            {
                _modifier = PlayerPrefs.GetInt("modifier");
                _level += _modifier;
                _modifier++;
                PlayerPrefs.SetInt("modifier", _modifier);
                PlayerPrefs.Save();
            }

            Debug.Log($"Level = {_level} Stars = {_stars}");
            
            PlayerPrefs.DeleteKey("level");
            PlayerPrefs.DeleteKey("stars");

            SetLevelStars();
        }
    }

    private void LoadLevels()
    {
        for (int i = 0; i < _levels.Length; i++)
        {
            int stars = PlayerPrefs.GetInt($"level_stars_{i}", 0);
            if (stars > 0)
            {
                Debug.Log($"level_stars_{i}");
                List<RectTransform> levelStars = GetTaggedChildren(_levels[i], "Star");

                for (int j = 0; j < stars && j < levelStars.Count; j++)
                {
                    Image starImage = levelStars[j].GetComponent<Image>();
                    levelStars[j].localScale = Vector3.one;
                    if (starImage != null)
                        starImage.DOFade(1f, 0f);
                }
            }
            
            if (PlayerPrefs.HasKey($"level_stars_{i}"))
            {
                
                List<RectTransform> pathElementss = GetTaggedChildren(_levelPath[i], "Path");
                List<RectTransform> noPathElementss = GetTaggedChildren(_levelPath[i], "NoPath");
                foreach (RectTransform rectTransform in noPathElementss)
                {
                    rectTransform.localScale = Vector3.zero;
                }
                foreach (var pathEl in pathElementss)
                {
                    pathEl.localScale = Vector3.one;

                    Image pathImage = pathEl.GetComponent<Image>();
                    if (pathImage != null)
                        pathImage.DOFade(1f, 0f);
                }
                Image lockImage = _levels[i + 1]
                    .GetComponentsInChildren<RectTransform>()
                    .FirstOrDefault(r => r.CompareTag("Lock"))
                    ?.GetComponent<Image>();

                if (lockImage != null)
                {
                    lockImage.gameObject.SetActive(false);
                }

                if (i > 0 && i - 1 < _levelPath.Length)
                {
                    List<RectTransform> pathElements = GetTaggedChildren(_levelPath[i - 1], "Path");
                    foreach (var pathEl in pathElements)
                    {
                        pathEl.localScale = Vector3.one;

                        Image pathImage = pathEl.GetComponent<Image>();
                        if (pathImage != null)
                            pathImage.DOFade(1f, 0f);
                    }
                }
            }
        }
    }

    private void SetLevelStars()
    {
        if (_level < 0 || _level >= _levels.Length)
            return;

        RectTransform currentLevel = _levels[_level];
        List<RectTransform> currentStars = GetTaggedChildren(currentLevel, "Star");

        if (currentStars.Count > _stars)
        {
            currentStars = currentStars.GetRange(0, _stars);
        }
        
        AnimateElements(currentStars, currentStars[0].localScale, 0.5f, true);

        if (PlayerPrefs.HasKey($"level_stars_{_level}")) 
            return;
        
        DOTween.Sequence().AppendInterval(currentStars.Count * 0.5f).OnComplete(() =>
        {
            if (_level < _levelPath.Length)
            {
                List<RectTransform> pathElements = GetTaggedChildren(_levelPath[_level], "Path");
                List<RectTransform> noPathElements = GetTaggedChildren(_levelPath[_level], "NoPath");
                AnimateElements(noPathElements, Vector3.zero, 0f);
                AnimateElements(pathElements, pathElements[0].localScale);

                DOTween.Sequence().AppendInterval(pathElements.Count * 0.5f).OnComplete(() =>
                {
                    if (_level + 1 < _levels.Length)
                    {
                        UnlockNextLevel(_level + 1);
                    }
                });
            }
        });

        PlayerPrefs.SetInt($"level_stars_{_level}", _stars);
        PlayerPrefs.SetInt("currentLevel", _level);
        PlayerPrefs.Save();
    }

    private void AnimateElements(List<RectTransform> elements, Vector3 targetScale, float duration = 0.5f, bool isStar = false)
    {
        Sequence sequence = DOTween.Sequence();

        foreach (RectTransform el in elements)
        {
            el.localScale = Vector3.zero;

            Image img = el.GetComponent<Image>();
            if (img != null)
                img.DOFade(1f, 0f);

            sequence.Append(el.DOScale(targetScale, duration).SetEase(Ease.OutBack).OnStart((() =>
            {
                if (isStar)
                {
                    int random = Random.Range(0, _starAppearSounds.Length);
                    _audioSource.PlayOneShot(_starAppearSounds[random]);
                }
                else
                {
                    int random = Random.Range(0, _pathAppearSounds.Length);
                    _audioSource.PlayOneShot(_pathAppearSounds[random]);
                }
            })));
        }
    }

    private void UnlockNextLevel(int levelIndex)
    {
        Image lockImage = _levels[levelIndex]
            .GetComponentsInChildren<RectTransform>()
            .FirstOrDefault(r => r.CompareTag("Lock"))
            ?.GetComponent<Image>();

        if (lockImage == null) return;

        Sequence explodeSequence = DOTween.Sequence();
        explodeSequence.Join(lockImage.rectTransform.DOScale(Vector3.one * 2f, 0.5f).SetEase(Ease.OutQuad)
            .OnStart((() =>
            {
                _audioSource.PlayOneShot(_lockAppearSound);
            })));
        explodeSequence.Join(lockImage.rectTransform.DORotate(new Vector3(0, 0, 180f), 0.5f, RotateMode.FastBeyond360).SetEase(Ease.OutCubic));
        explodeSequence.Join(lockImage.DOFade(0f, 0.5f).SetEase(Ease.InQuad));
        explodeSequence.OnComplete(() => lockImage.gameObject.SetActive(false));
    }

    private List<RectTransform> GetTaggedChildren(RectTransform parent, string tag)
    {
        return parent.GetComponentsInChildren<RectTransform>()
                     .Where(t => t.CompareTag(tag))
                     .ToList();
    }
}
