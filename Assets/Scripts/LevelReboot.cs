using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class LevelReboot : MonoBehaviour
{
    public PlayerController playerController;

    void Update() 
    {
        if(playerController.currentHealth < 0)
        {
            Invoke("RebootTheLevel",1);
        }
    }
    void RebootTheLevel()
    {
        SceneManager.LoadScene(3);
    }

}
