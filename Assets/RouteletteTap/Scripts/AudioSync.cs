using UnityEngine;

public class AudioSync : MonoBehaviour
{
    [Header("References")]
    [SerializeField] private AudioSource _audioSource;
    [SerializeField] private AudioClip _music;
    [SerializeField] private RouletteController _rouletteController;

    [Header("Spectrum Settings")]
    [SerializeField] private int spectrumSize = 64;
    [SerializeField] private FFTWindow fftWindow = FFTWindow.BlackmanHarris;

    [Header("Punch Settings (Low Frequencies)")]
    [SerializeField] private Vector3 punchScale = new Vector3(0.2f, 0.2f, 0.2f);
    [SerializeField] private float punchDuration = 0.2f;
    [SerializeField] private int punchFrequencyRangeStart = 0;
    [SerializeField] private int punchFrequencyRangeEnd = 10;
    [SerializeField] private float punchSensitivity = 20f;
    [SerializeField] private float punchCooldown = 0.2f;

    [Header("Spin Boost Settings (Mid Frequencies)")]
    [SerializeField] private float speedBoostAmount = 50f;
    [SerializeField] private float speedBoostDuration = 0.4f;
    [SerializeField] private int speedFrequencyRangeStart = 11;
    [SerializeField] private int speedFrequencyRangeEnd = 30;
    [SerializeField] private float speedSensitivity = 20f;
    [SerializeField] private float speedCooldown = 0.3f;
    
    [Header("Reverse Spin Settings (High Frequencies)")]
    [SerializeField] private int reverseFrequencyStart = 40;
    [SerializeField] private int reverseFrequencyEnd = 63;
    [SerializeField] private float reverseSensitivity = 15f;
    [SerializeField] private float reverseThreshold = 1f;
    [SerializeField] private float reverseCooldown = 0.8f;

    private bool _isStart;
    
    private float[] _spectrum;
    private float _punchTimer = 0f;
    private float _speedTimer = 0f;
    private float _reverseTimer = 0f;

    private void Start()
    {
        _spectrum = new float[spectrumSize];
    }

    private void Update()
    {
        if (!_isStart)
            return;
        if (!_rouletteController.IsSpinning()) 
            return;

        _punchTimer += Time.deltaTime;
        _speedTimer += Time.deltaTime;
        _reverseTimer += Time.deltaTime;

        _audioSource.GetSpectrumData(_spectrum, 0, fftWindow);

        float reverseAmp = GetAmplitude(reverseFrequencyStart, reverseFrequencyEnd) * reverseSensitivity;

        float punchAmp = GetAmplitude(punchFrequencyRangeStart, punchFrequencyRangeEnd) * punchSensitivity;
        float speedAmp = GetAmplitude(speedFrequencyRangeStart, speedFrequencyRangeEnd) * speedSensitivity;

        if (punchAmp > 1f && _punchTimer >= punchCooldown)
        {
            _rouletteController.PunchEffect(punchScale, punchDuration);
            _punchTimer = 0f;
        }

        if (speedAmp > 1f && _speedTimer >= speedCooldown)
        {
            _rouletteController.TempSpeedBoost(speedBoostAmount, speedBoostDuration);
            _speedTimer = 0f;
        }
        
        if (reverseAmp > reverseThreshold && _reverseTimer >= reverseCooldown)
        {
            _rouletteController.ReverseRotation(0.5f);
            _reverseTimer = 0f;
        }
    }

    private float GetAmplitude(int start, int end)
    {
        float sum = 0f;
        for (int i = start; i <= end && i < _spectrum.Length; i++)
        {
            sum += _spectrum[i];
        }
        return sum;
    }

    public void Initialize()
    {
        _audioSource.clip = _music;
        _audioSource.Play();
        _isStart = true;
    }
}