using UnityEngine;

public class AudioController : MonoBehaviour
{
    [SerializeField] private AudioClip _clickSound;
    
    private AudioSource _audioSource;

    private void Start()
    {
        _audioSource = GetComponent<AudioSource>();
    }

    public void PlayClickSound()
    {
        _audioSource.PlayOneShot(_clickSound);   
    }
}
