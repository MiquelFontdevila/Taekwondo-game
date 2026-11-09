using UnityEngine;
using UnityEngine.UI;

[RequireComponent(typeof(Button))]
public class ButtonSound : MonoBehaviour
{
    private Button boto;
    private AudioManager audioManager;

    void Awake()
    {
        boto = GetComponent<Button>();
    }

    void OnEnable()
    {
        boto.onClick.AddListener(Sonar);
    }

    void OnDisable()
    {
        boto.onClick.RemoveListener(Sonar);
    }

    private void Sonar()
    {
        if (audioManager == null)
        {
            GameObject audioGO = GameObject.FindGameObjectWithTag("Audio");
            if (audioGO != null)
            {
                audioManager = audioGO.GetComponent<AudioManager>();
            }
        }

        if (audioManager != null)
        {
            audioManager.PlaySFX(audioManager.buttons_UI);
        }
    }
}