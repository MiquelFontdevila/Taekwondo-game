using UnityEngine;
using UnityEngine.SceneManagement;

public class LoadPauseScene : MonoBehaviour
{
    [SerializeField] private GameObject pauseMenuUI;
    public void Pause()
    {
        pauseMenuUI.SetActive(true);
    }

    public void LoadMainMenu()
    {
        SceneManager.LoadScene("MainMenuScene");
    }

    public void Resume()
    {
        pauseMenuUI.SetActive(false);
    }

    public void Restart()
    {
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
    }
}
