using UnityEngine;

public class KillerObjects : MonoBehaviour
{

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.gameObject.CompareTag("Player"))
        {
            FindObjectOfType<PlayerController>().TakeDamage(FindObjectOfType<PlayerController>().currentHealth);
        }
    }
}
