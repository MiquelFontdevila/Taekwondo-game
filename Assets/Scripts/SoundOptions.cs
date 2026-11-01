using UnityEngine;
using UnityEngine.Audio;
using UnityEngine.UI;

public class SoundOptions : MonoBehaviour
{
    public Slider sliderMusicVol, sliderSfxVol;
    public AudioMixer mainAudioMixer;

    public void ChangeMusicVolume()
    {
        mainAudioMixer.SetFloat("MusicVol", sliderMusicVol.value);
    }

    public void ChangeSfxVolume()
    {
        mainAudioMixer.SetFloat("SFXVol", sliderSfxVol.value);
    }
}
