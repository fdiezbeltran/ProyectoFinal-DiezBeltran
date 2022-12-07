using UnityEngine;

public class Fireball : MonoBehaviour
{
    public Animator animator;
    public int attackDamage = 50;

    private void OnTriggerEnter2D(Collider2D collision) 
    {
        if (collision.gameObject.CompareTag("Player"))
        {
            animator.SetTrigger("Explote");
            Destroy(gameObject);
        }
        if (collision.gameObject.CompareTag("Level"))
        {
            animator.SetTrigger("Explote");
            Destroy(gameObject);
        }
    }
}
