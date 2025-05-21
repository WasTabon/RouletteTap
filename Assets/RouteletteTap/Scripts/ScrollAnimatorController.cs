using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;
using System.Collections.Generic;

public class ScrollViewAnimator : MonoBehaviour
{
    public ScrollRect scrollRect;
    public RectTransform viewport;

    [Header("Настройки анимации")]
    public float appearScaleDuration = 0.4f;
    public float appearScaleDelay = 0.05f;
    public float scrollFollowDelay = 0.15f;
    public float scrollSpeedThreshold = 10f;

    [Header("Анимируемые объекты")]
    public List<RectTransform> levelsAndStars;
    public List<RectTransform> pathObjects;

    private Dictionary<RectTransform, Vector3> originalPositions = new Dictionary<RectTransform, Vector3>();
    private Dictionary<RectTransform, Vector3> originalScales = new Dictionary<RectTransform, Vector3>();
    private Dictionary<RectTransform, Vector3> velocities = new Dictionary<RectTransform, Vector3>();
    private HashSet<RectTransform> appeared = new HashSet<RectTransform>();

    private Vector2 lastContentAnchoredPos;
    private Vector2 lastScrollPos;
    private bool isScrolling = false;

    private string scrollSaveKey = "ScrollView_Position";

    void Start()
    {
        // Загрузка сохранённой позиции
        LoadScrollPosition();

        lastScrollPos = scrollRect.content.anchoredPosition;
        lastContentAnchoredPos = scrollRect.content.anchoredPosition;

        InitObjects(levelsAndStars);
        InitObjects(pathObjects);
    }

    void InitObjects(List<RectTransform> objects)
    {
        foreach (var obj in objects)
        {
            originalPositions[obj] = obj.localPosition;
            originalScales[obj] = obj.localScale;
            velocities[obj] = Vector3.zero;

            if (IsVisible(obj))
            {
                appeared.Add(obj);
            }
            else
            {
                obj.localScale = Vector3.zero;
            }
        }
    }

    void Update()
    {
        Vector2 currentPos = scrollRect.content.anchoredPosition;
        float speed = (currentPos - lastScrollPos).magnitude / Time.deltaTime;
        lastScrollPos = currentPos;

        isScrolling = speed > scrollSpeedThreshold;

        AnimateVisibleObjects(levelsAndStars);
        AnimateVisibleObjects(pathObjects);

        lastContentAnchoredPos = scrollRect.content.anchoredPosition;
    }

    void AnimateVisibleObjects(List<RectTransform> objects)
    {
        foreach (var obj in objects)
        {
            if (IsVisible(obj))
            {
                if (!appeared.Contains(obj))
                {
                    if (!originalScales.ContainsKey(obj))
                        originalScales[obj] = obj.localScale == Vector3.zero ? Vector3.one : obj.localScale;

                    obj.localScale = Vector3.zero;

                    obj.DOScale(originalScales[obj], appearScaleDuration)
                        .SetEase(Ease.OutBack)
                        .SetDelay(appearScaleDelay);

                    appeared.Add(obj);
                }
                else
                {
                    if (obj.localScale != originalScales[obj])
                    {
                        originalScales[obj] = obj.localScale;
                    }
                }

                Vector3 targetPosition = originalPositions[obj] + (Vector3)(scrollRect.content.anchoredPosition - lastContentAnchoredPos);
                Vector3 velocity = velocities[obj];

                obj.localPosition = Vector3.SmoothDamp(
                    obj.localPosition,
                    targetPosition,
                    ref velocity,
                    scrollFollowDelay
                );

                velocities[obj] = velocity;
            }
            else
            {
                if (appeared.Contains(obj))
                {
                    obj.DOScale(Vector3.zero, appearScaleDuration)
                        .SetEase(Ease.InBack);
                    appeared.Remove(obj);
                }
            }
        }
    }

    bool IsVisible(RectTransform obj)
    {
        Rect viewportRect = GetWorldRect(viewport);
        Rect objRect = GetWorldRect(obj);
        return viewportRect.Overlaps(objRect, true);
    }

    Rect GetWorldRect(RectTransform rectTransform)
    {
        Vector3[] corners = new Vector3[4];
        rectTransform.GetWorldCorners(corners);
        Vector3 bottomLeft = corners[0];
        Vector3 topRight = corners[2];
        return new Rect(bottomLeft, topRight - bottomLeft);
    }
    
    public void SaveScrollPosition()
    {
        Vector2 pos = scrollRect.content.anchoredPosition;
        PlayerPrefs.SetFloat(scrollSaveKey + "_x", pos.x);
        PlayerPrefs.SetFloat(scrollSaveKey + "_y", pos.y);
        PlayerPrefs.Save();
    }

    void LoadScrollPosition()
    {
        if (PlayerPrefs.HasKey(scrollSaveKey + "_x") && PlayerPrefs.HasKey(scrollSaveKey + "_y"))
        {
            float x = PlayerPrefs.GetFloat(scrollSaveKey + "_x");
            float y = PlayerPrefs.GetFloat(scrollSaveKey + "_y");
            scrollRect.content.anchoredPosition = new Vector2(x, y);
        }
    }
}
