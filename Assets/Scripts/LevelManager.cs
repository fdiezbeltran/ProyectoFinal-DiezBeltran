using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class LevelManager : MonoBehaviour
{
    //public Rigidbody2D rb;

    private void OnTriggerEnter2D(Collider2D col) 
    {
            if(col.gameObject.CompareTag("Player"))
            {
                SceneManager.LoadScene(2);
            }
    }

}
