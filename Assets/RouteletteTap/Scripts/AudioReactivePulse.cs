using UnityEngine;

public class AudioReactivePulse : MonoBehaviour
{
    [Header("Audio Settings")]
    [SerializeField] private AudioSync audioSync;
    [SerializeField] private int frequencyRangeStart = 0;
    [SerializeField] private int frequencyRangeEnd = 15;
    [SerializeField] private float sensitivity = 20f;
    [SerializeField] private float pulseThreshold = 1f;

    [Header("Pulse Settings")]
    [SerializeField] private Vector3 baseScale = Vector3.one;
    [SerializeField] private Vector3 pulseScale = new Vector3(1.1f, 1.1f, 1f);
    [SerializeField] private float lerpSpeed = 5f;

    private RectTransform _rectTransform;

    private void Awake()
    {
        _rectTransform = GetComponent<RectTransform>();
    }

    private void Update()
    {
        if (!audioSync._isStart)
            return;
        if (audioSync == null)
            return;
        if (!audioSync.gameObject.activeSelf)
            return;

        float amplitude = audioSync.GetAmplitude(frequencyRangeStart, frequencyRangeEnd) * sensitivity;

        Vector3 targetScale = (amplitude > pulseThreshold) ? pulseScale : baseScale;
        _rectTransform.localScale = Vector3.Lerp(_rectTransform.localScale, targetScale, Time.deltaTime * lerpSpeed);
    }
}