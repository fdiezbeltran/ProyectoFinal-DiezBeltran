using UnityEngine;

public class RangedAttack : MonoBehaviour
{
    public ParticleSystem arrowHit;

    private void OnTriggerEnter2D(Collider2D collision) 
    {
        if (collision.gameObject.CompareTag("Enemy"))
        {
            arrowHit.Play();
            Destroy(gameObject);
            
        }
        if (collision.gameObject.CompareTag("Level"))
        {
            Destroy(gameObject);
            
        }
    }
    
}
