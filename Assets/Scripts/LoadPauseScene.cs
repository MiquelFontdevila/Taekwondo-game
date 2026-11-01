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

    public void Pause()
    {
        pauseMenuUI.SetActive(true);
        pausaButton.interactable = false;
        pauseMenuCanvasGroup.interactable = false;
        EventSystem.current.SetSelectedGameObject(primerButton.gameObject);
        combatUI.pausat = true;
    }

    public void LoadMainMenu()
    {
        SceneManager.LoadScene("MainMenuScene");
    }

    public void Resume()
    {
        pauseMenuUI.SetActive(false);
        pausaButton.interactable = true;
        pauseMenuCanvasGroup.interactable = true;
        
        combatUI.pausat = false;
        combatUI.RestaurarSeleccio();
        
    }

    public void Restart()
    {
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
    }
}
