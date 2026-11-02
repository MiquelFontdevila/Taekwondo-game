using UnityEngine;

public class AudioManager : MonoBehaviour
{
    [Header("Audio Sorce")]
    [SerializeField] AudioSource musicSource;
    // [SerializeField] AudioSource SFXSource;

    [Header("Audio Clip")]
    public AudioClip background;

    private void Start()
    {
        musicSource.clip = background;
        // musicSource.loop = true;
        musicSource.Play();
    }
}
