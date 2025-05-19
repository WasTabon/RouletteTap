using System;
using System.Collections;
using DG.Tweening;
using UnityEngine;
using Random = UnityEngine.Random;

public class RouletteController : MonoBehaviour
{
    public event Action OnStartSpin;
    
    [SerializeField] public float _rotateSpeed;
    [SerializeField] private Transform[] _buttons;

    [SerializeField] private float _wheelAnimationSpeed;
    [SerializeField] private float _buttonAnimationSpeed;

    [SerializeField] private Ease _wheelEase;
    [SerializeField] private Ease _buttonEase;

    [SerializeField] private AudioClip _wheelSound;
    [SerializeField] private AudioClip _buttonSound;

    [SerializeField] private GameObject _buttonSpawanParticle;
    
    private AudioSource _audioSource;
    
    private Vector3 _rotateDirection;
    private Vector3 _wheelSize;
    private Vector3 _buttonSize;
    
    private bool _isSpin;

    private void Start()
    {
        _audioSource = GetComponent<AudioSource>();
        
        _wheelSize = gameObject.transform.localScale;
        _buttonSize = _buttons[0].localScale;

        gameObject.transform.localScale = Vector3.zero;
        foreach (Transform button in _buttons)
        {
            button.localScale = Vector3.zero;
        }
        
        SetupWheel();
        ChangeRotateSpeed(_rotateSpeed);
    }

    private void Update()
    {
        _rotateDirection = new Vector3(0, 0, _rotateSpeed) * -1f;
        if (_isSpin)
        {
               
            HandleRotate();
        }
    }

    public void StartSpin() => _isSpin = true;

    public void StopSpin() => _isSpin = false;

    public Transform[] GetButtons() => _buttons;
    
    public void ChangeRotateSpeed(float speed)
    {
        _rotateDirection = new Vector3(0, 0, speed) * -1f;
    }

    private void HandleRotate()
    {
        transform.Rotate(_rotateDirection * Time.deltaTime);
    }

    private void SetupWheel()
    {
        gameObject.transform.DOScale(_wheelSize, _wheelAnimationSpeed)
            .SetEase(_wheelEase)
            .OnPlay((() =>
            {
                _audioSource.PlayOneShot(_wheelSound);
            }))
            .OnComplete(() =>
            {
                Sequence sequence = DOTween.Sequence();

                foreach (Transform button in _buttons)
                {
                    sequence.Append(
                        button.DOScale(_buttonSize, _buttonAnimationSpeed)
                            .SetEase(_buttonEase)
                            .OnStart((() =>
                            {
                                _audioSource.PlayOneShot(_buttonSound);
                                _audioSource.pitch += 0.0833f;
                                GameObject particle = Instantiate(_buttonSpawanParticle, button.position, Quaternion.identity);
                                StartCoroutine(DeactivateAfterDelay(particle, 3f));
                            }))
                    );
                }

                sequence.OnComplete(() =>
                {
                    _audioSource.pitch = 1;
                    OnStartSpin?.Invoke();
                });
            });
    }

    public bool IsSpinning() => _isSpin;

    public void PunchEffect(Vector3 punchStrength, float duration)
    {
        transform.DOKill();
        transform.localScale = _wheelSize;
        transform.DOPunchScale(punchStrength, duration, 10, 1);
    }

    public void TempSpeedBoost(float amount, float duration)
    {
        //int random = Random.Range(0, 100);
        //if (random <= 35)
            //amount *= -1;
        
        float originalSpeed = _rotateSpeed;
        _rotateSpeed += amount;

        DOVirtual.DelayedCall(duration, () =>
        {
            _rotateSpeed = originalSpeed;
        });
    }
    
    public void ReverseRotation(float duration)
    {
        _rotateSpeed *= -1;
        DOVirtual.DelayedCall(duration, () => _rotateSpeed *= -1);
    }
    
    private IEnumerator DeactivateAfterDelay(GameObject particle, float delay)
    {
        yield return new WaitForSeconds(delay);
        DeactivateParticle(particle);
    }

    private void DeactivateParticle(GameObject particle)
    {
        if (particle != null)
            particle.SetActive(false);
    }
}