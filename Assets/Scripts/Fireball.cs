using UnityEngine;
using UnityEngine.Rendering.Universal;

public class Fireball : MonoBehaviour
{   
    public Rigidbody2D rb;
    public Animator animator;
    public UnityEngine.Rendering.Universal.Light2D fireballLight;
    public int attackDamage = 50;
    bool lightFade;

    void Update() 
    {
        if(lightFade)
        {
            fireballLight.intensity -= 0.5f;
        }
    }


    private void OnTriggerEnter2D(Collider2D collision) 
    {
        if (collision.gameObject.CompareTag("Player"))
        {
            animator.SetTrigger("explote");
            rb.velocity = new Vector2(0, 0);
            Destroy(gameObject, 0.5f);
            lightFade = true;
        }
        if (collision.gameObject.CompareTag("Level"))
        {
            animator.SetTrigger("explote");
            rb.velocity = new Vector2(0, 0);
            Destroy(gameObject, 0.5f);
            lightFade = true;
        }
    }
}
