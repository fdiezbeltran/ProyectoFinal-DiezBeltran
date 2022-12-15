using UnityEngine;
using UnityEngine.SceneManagement;

public class EndGame : MonoBehaviour
{
    public void EndGameLevel()
    {
        Invoke("ChangeToLastScene", 3);
    }

    public void ChangeToLastScene()
    {
        SceneManager.LoadScene(4);
        Debug.Log("esta invokando el final");
    }
}
