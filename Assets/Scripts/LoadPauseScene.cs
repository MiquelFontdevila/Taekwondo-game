using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using UnityEngine.SceneManagement;

public class LoadPauseScene : MonoBehaviour
{
    [SerializeField] private GameObject pauseMenuUI;
    [SerializeField] private Button pausaButton;
    [SerializeField] private CanvasGroup pauseMenuCanvasGroup;
    [SerializeField] private Button primerButton;
    [SerializeField] private CombatUIManager combatUI;
    [SerializeField] private GameObject marcador;
    [SerializeField] private GameObject backgroundPausa;
    AudioManager audioManager;


    void Start()
    {
        GameObject audioGO = GameObject.FindGameObjectWithTag("Audio");
        if (audioGO != null)
        {
            audioManager = audioGO.GetComponent<AudioManager>();
        }
    }

    public void LoadMainMenu()
    {
        //audioManager.ReprendreMusica();
        SceneManager.LoadScene("MainMenuScene");
    }

        public void Pause()
    {
        pauseMenuUI.SetActive(true);
        pausaButton.interactable = false;
        pauseMenuCanvasGroup.interactable = false;
        EventSystem.current.SetSelectedGameObject(primerButton.gameObject);
        combatUI.pausat = true;
        marcador.SetActive(false);
        backgroundPausa.SetActive(true);
        if (audioManager != null) audioManager.PausarMusica();
    }

    public void Resume()
    {
        marcador.SetActive(true);
        pauseMenuUI.SetActive(false);
        pausaButton.interactable = true;
        pauseMenuCanvasGroup.interactable = true;
        backgroundPausa.SetActive(false);

        combatUI.pausat = false;
        combatUI.RestaurarSeleccio();

        if (audioManager != null) audioManager.ReprendreMusica();
    }

    public void Restart()
    {
        //audioManager.ReprendreMusica();
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
    }
}
