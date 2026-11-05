using System.Collections;
using UnityEngine;

// Tipus de so que pot demanar el sistema de batalla.
// L'AudioManager tria el clip concret segons el tag del lluitador.
public enum TipusSo
{
    AtacAlt,
    AtacCos,
    Bloqueig,
    Esquiva,
    VeuEmpenta,
    VeuRebutCop
}

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

    [Header("Veus noi")]
    public AudioClip Atac_alt_noi;
    public AudioClip Atac_cos_noi;
    public AudioClip Bloqueig_noi;
    public AudioClip Esquiva_noi;
    public AudioClip Veus_Empendra_noi;
    public AudioClip Veus_Rebuts_cop_noi;

    [Header("Veus noia")]
    public AudioClip Atac_alt_noia;
    public AudioClip Atac_cos_noia;
    public AudioClip Bloqueig_noia;
    public AudioClip Esquiva_noia;
    public AudioClip Veus_Empendra_noia;
    public AudioClip Veus_Rebuts_cop_noia;

    [Header("Veus Rival")]
    public AudioClip Atac_alt_rival;
    public AudioClip Atac_cos_rival;
    public AudioClip Bloqueig_rival;
    public AudioClip Esquiva_rival;
    public AudioClip Veus_Empendra_rival;
    public AudioClip Veus_Rebuts_cop_rival;

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

    // ---------- Sons segons el tag del lluitador ----------

    // Reprodueix a l'instant el so d'un tipus per al lluitador amb aquest tag
    public void PlaySFX(TipusSo tipus, string etiqueta)
    {
        PlaySFX(ObtenirClip(tipus, etiqueta));
    }

    // Reprodueix amb retard el so d'un tipus per al lluitador amb aquest tag
    public void PlaySFXDelayed(TipusSo tipus, string etiqueta, float delay)
    {
        PlaySFXDelayed(ObtenirClip(tipus, etiqueta), delay);
    }
    
    // ---------- Sons segons el tag del lluitador ----------

    // Reprodueix a l'instant el so d'un tipus per al lluitador amb aquest tag
    public void PlaySFX(TipusSo tipus, string etiqueta)
    {
        PlaySFX(ObtenirClip(tipus, etiqueta));
    }

    // Reprodueix amb retard el so d'un tipus per al lluitador amb aquest tag
    public void PlaySFXDelayed(TipusSo tipus, string etiqueta, float delay)
    {
        PlaySFXDelayed(ObtenirClip(tipus, etiqueta), delay);
    }

    // Retorna el clip que toca segons el tipus de so i el tag
    public AudioClip ObtenirClip(TipusSo tipus, string etiqueta)
    {
        switch (etiqueta)
        {
            case "JugadorNoi":  return ClipNoi(tipus);
            case "JugadorNoia": return ClipNoia(tipus);
            case "Rival":       return ClipRival(tipus);
            default:
                Debug.LogWarning("AudioManager: tag desconegut '" + etiqueta + "'");
                return null;
        }
    }

    private AudioClip ClipNoi(TipusSo tipus) => tipus switch
    {
        TipusSo.AtacAlt     => Atac_alt_noi,
        TipusSo.AtacCos     => Atac_cos_noi,
        TipusSo.Bloqueig    => Bloqueig_noi,
        TipusSo.Esquiva     => Esquiva_noi,
        TipusSo.VeuEmpenta  => Veus_Empendra_noi,
        TipusSo.VeuRebutCop => Veus_Rebuts_cop_noi,
        _ => null
    };

    private AudioClip ClipNoia(TipusSo tipus) => tipus switch
    {
        TipusSo.AtacAlt     => Atac_alt_noia,
        TipusSo.AtacCos     => Atac_cos_noia,
        TipusSo.Bloqueig    => Bloqueig_noia,
        TipusSo.Esquiva     => Esquiva_noia,
        TipusSo.VeuEmpenta  => Veus_Empendra_noia,
        TipusSo.VeuRebutCop => Veus_Rebuts_cop_noia,
        _ => null
    };

    private AudioClip ClipRival(TipusSo tipus) => tipus switch
    {
        TipusSo.AtacAlt     => Atac_alt_rival,
        TipusSo.AtacCos     => Atac_cos_rival,
        TipusSo.Bloqueig    => Bloqueig_rival,
        TipusSo.Esquiva     => Esquiva_rival,
        TipusSo.VeuEmpenta  => Veus_Empendra_rival,
        TipusSo.VeuRebutCop => Veus_Rebuts_cop_rival,
        _ => null
    };
}
