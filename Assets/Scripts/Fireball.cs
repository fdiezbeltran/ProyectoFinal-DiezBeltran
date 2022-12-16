using UnityEngine;

public class Fireball : MonoBehaviour
{   
    public Rigidbody2D rb;
    public Animator animator;
    public int attackDamage = 50;
    
    void Update() 
    {
        
    }


    private void OnTriggerEnter2D(Collider2D collision) 
    {
        if (collision.gameObject.CompareTag("Player"))
        {
            animator.SetTrigger("explote");
            rb.velocity = new Vector2(0, 0);
            Destroy(gameObject, 0.5f);
        }
        if (collision.gameObject.CompareTag("Level"))
        {
            animator.SetTrigger("explote");
            rb.velocity = new Vector2(0, 0);
            Destroy(gameObject, 0.5f);
        }
    }
}
