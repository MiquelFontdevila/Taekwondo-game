
using UnityEngine;
using UnityEngine.Audio;
using UnityEngine.UI;

public class SoundOptions : MonoBehaviour
{
    [Header("Sliders")]
    public Slider sliderMusicVol;
    public Slider sliderSfxVol;

    [Header("Audio Mixer")]
    public AudioMixer mainAudioMixer;

    private const string MusicKey = "MusicVolume";
    private const string SfxKey = "SFXVolume";

    private void Start()
    {
        float musica = PlayerPrefs.GetFloat(MusicKey, 0.65f);
        float sfx = PlayerPrefs.GetFloat(SfxKey, 0.75f);

        sliderMusicVol.SetValueWithoutNotify(musica);
        sliderSfxVol.SetValueWithoutNotify(sfx);

        AplicarVolums(musica, sfx);
    }

    public void ChangeMusicVolume()
    {
        float valor = sliderMusicVol.value;

        AplicarVolum("MusicVol", valor);
        PlayerPrefs.SetFloat(MusicKey, valor);
    }

    public void ChangeSfxVolume()
    {
        float valor = sliderSfxVol.value;

        AplicarVolum("SFXVol", valor);
        PlayerPrefs.SetFloat(SfxKey, valor);
    }

    private void AplicarVolums(float musica, float sfx)
    {
        AplicarVolum("MusicVol", musica);
        AplicarVolum("SFXVol", sfx);
    }

    private void AplicarVolum(string parametre, float valor)
    {
        if (valor <= 0.0001f)
        {
            mainAudioMixer.SetFloat(parametre, -80f);
            return;
        }

        float db = 20f * Mathf.Log10(valor);

        mainAudioMixer.SetFloat(parametre, db);
    }

    public void GuardarConfiguracio()
    {
        PlayerPrefs.Save();
    }
}