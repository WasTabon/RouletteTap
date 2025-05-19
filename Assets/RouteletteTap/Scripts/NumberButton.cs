using System;
using System.Collections;
using TMPro;
using UnityEngine;

public class NumberButton : MonoBehaviour
{
    public event Action OnGood;
    public event Action OnBad;
    
    [SerializeField] private int _id;
    [SerializeField] private TextMeshPro _text;

    [SerializeField] public Transform _particle;
    
    [SerializeField] private GameObject _correctParticle;
    [SerializeField] private GameObject _incorrectParticle;

    [SerializeField] private float _correctTapVolume;
    [SerializeField] private float _incorrectTapVolume;
    [SerializeField] private AudioClip _tapCorrectSound;
    [SerializeField] private AudioClip _tapIncorrectSound;
    
    [SerializeField] private Transform _center;

    private AudioSource _audioSource;
    
    public bool isClickable;

    private void Start()
    {
        _audioSource = GetComponent<AudioSource>();
        _text.text = _id.ToString();
        _particle.gameObject.SetActive(false);
        LookAt(_center);
    }

    private void Update()
    {
        //HandleTap();
    }
    
    public void OnTap()
    {
        if (isClickable)
        {
            isClickable = false;
            _audioSource.PlayOneShot(_tapCorrectSound, _correctTapVolume);
            GameObject particle = Instantiate(_correctParticle, transform.position, Quaternion.identity);
            StartCoroutine(DeactivateAfterDelay(particle, 3f));
            OnGood?.Invoke();
        }
        else
        {
            _audioSource.PlayOneShot(_tapIncorrectSound, _incorrectTapVolume);
            GameObject particle = Instantiate(_incorrectParticle, transform.position, Quaternion.identity);
            StartCoroutine(DeactivateAfterDelay(particle, 3f));
            OnBad?.Invoke();
        }
    }

    private void LookAt(Transform cener)
    {
        Vector3 direction = cener.position - transform.position;
        
        float angle = Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg;

        transform.rotation = Quaternion.Euler(0, 0, angle);
    }
    
    private IEnumerator DeactivateAfterDelay(GameObject particle, float delay)
    {
        float timer = 0f;
    
        while (timer < delay)
        {
            if (particle == null) yield break;

            particle.transform.position = transform.position;

            timer += Time.deltaTime;
            yield return null;
        }

        DeactivateParticle(particle);
    }

    private void DeactivateParticle(GameObject particle)
    {
        if (particle != null)
            particle.SetActive(false);
    }
    
    private void OnValidate()
    {
        gameObject.name = $"NumberButton {_id}";
    }
}
