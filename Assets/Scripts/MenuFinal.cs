using UnityEngine;
using UnityEngine.SceneManagement;

public class MenuFinal : MonoBehaviour
{
    GameObject MusicManager;

    public void Salir()
    {
        SceneManager.LoadScene(0);
        MusicManager = GameObject.Find("MusicManager");
        Destroy(MusicManager);
    }
}
