using System.Collections;
using UnityEngine;

public class AudioManager : MonoBehaviour
{

    //Per trobar rapidament el comoponent de audioManager podem fer aixo:
    //AudioManager = GameObject.FindGameObjectWithTag("Audio").GetComponent<AudioManager>();

    [Header("Audio Sorce")]
    [SerializeField] AudioSource musicSource;
    [SerializeField] AudioSource SFXSource;

    [Header("Audio Clip")]
    public AudioClip background;

    public AudioClip buttons_UI;
    public AudioClip Pases;
    public AudioClip Atac_1;
    public AudioClip Atac_2;
    public AudioClip Bloqueig;
    public AudioClip Esquiva;
    public AudioClip Veus_Bloqueig;
    public AudioClip Veus_Cops_Alts;
    public AudioClip Veus_Cops_Baixos;
    public AudioClip Veus_Rebuts_Alts;
    public AudioClip Veus_Rebuts_Baixos;

    private void Start()
    {
        musicSource.clip = background;
        // musicSource.loop = true;
        musicSource.Play();
    }
    // Reprodueix un so d'efecte a l'instant
    public void PlaySFX(AudioClip clip)
    {
        if (clip == null) return;
        SFXSource.PlayOneShot(clip);
    }

    // Reprodueix un so d'efecte després d'uns segons
    public void PlaySFXDelayed(AudioClip clip, float delay)
    {
        StartCoroutine(PlaySFXDelayedRoutine(clip, delay));
    }

    private IEnumerator PlaySFXDelayedRoutine(AudioClip clip, float delay)
    {
        yield return new WaitForSeconds(delay);
        PlaySFX(clip);
    }
}
