using UnityEngine;
using UnityEngine.SceneManagement;

public class LevelManager : MonoBehaviour
{
    public int nextLevel;

    private void OnTriggerEnter2D(Collider2D col) 
    {
            if(col.gameObject.CompareTag("Player"))
            {
                SceneManager.LoadScene(nextLevel);
            }
    }

}
