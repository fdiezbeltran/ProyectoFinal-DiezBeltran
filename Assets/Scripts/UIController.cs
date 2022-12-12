using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class UIController : MonoBehaviour
{
    
    void Update()
    {
        UpdateHUD();
        TogglePause();
        DialogueHideHUD();
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
    
        bossCurrentHealth = finalBoss.GetComponent<Enemy>().currentHealth;
        bossLifeBar.fillAmount =  bossCurrentHealth / 1000;
        
    }
    [Space]
    [Header("Barra de vida Jefe")]
    public GameObject finalBoss;
    public Image bossLifeBar;
    public float bossCurrentHealth;



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
        pauseMenu.SetActive(true);
        Time.timeScale = 0;
    }
    public void ResumeGame()
    {
        gamePaused = false;
        pauseMenu.SetActive(false);
        Time.timeScale = 1;
    }
    public void ExitToMenu()
    {
        SceneManager.LoadScene(0);
    }

    [Space]
    [Header("Dialogue Playing")]
    public DialogueManager dialogueManager;

    void DialogueHideHUD()
    {
        if(dialogueManager.dialogueIsPlaying)
        {
            hud.SetActive(false);
        }else
        {
            hud.SetActive(true);
        }
    }

}
