using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class UIController : MonoBehaviour
{
    void Update()
    {
        UpdateHUD();
        TogglePause();
    }

    [Header("Barra de vida")]
    public GameObject hud;
    public Image lifeBar;
    public float currentHealth;
    public int uiMaxHealth;

    void UpdateHUD()
    {
        uiMaxHealth = PlayerController.playerMaxHealth;
        currentHealth = PlayerController.uiHealth;
        lifeBar.fillAmount =  currentHealth / uiMaxHealth;
    }
    [Space]
    [Header("Pausa")]
    public GameObject pauseMenu;
    public bool gamePaused = false;
    void TogglePause()
    {
        if(Input.GetKeyDown("escape"))
        {
            if(!gamePaused)
            {
                PauseGame();
            }else
            {
                ResumeGame();
            }
        }
    }
    public void PauseGame()
    {
        gamePaused = true;
        hud.SetActive(false);
        pauseMenu.SetActive(true);
        Time.timeScale = 0;
    }
    public void ResumeGame()
    {
        gamePaused = false;
        hud.SetActive(true);
        pauseMenu.SetActive(false);
        Time.timeScale = 1;
    }
    public void ExitToMenu()
    {
        SceneManager.LoadScene(0);
    }
}
