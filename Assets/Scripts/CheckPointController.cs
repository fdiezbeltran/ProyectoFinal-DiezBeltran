using UnityEngine;

public class CheckPointController : MonoBehaviour
{
    public Transform Spawn;
    public GameObject checkPointSprite;
    public GameObject checkPointActivated;

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.gameObject.CompareTag("Player"))
        {
            Spawn.transform.position = transform.position;
            checkPointSprite.SetActive(false);
            checkPointActivated.SetActive(true);
        }
    }
}
