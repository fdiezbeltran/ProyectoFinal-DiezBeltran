using System.Collections;
using UnityEngine;

public class Enemy : MonoBehaviour
{
    public Animator animator;
    public Rigidbody2D rb;
    public SpriteRenderer sp;
    public PlayerController playerController;
    public AudioSource audioSource;
    

    public int maxHealth = 100;
    public int currentHealth;
    public int attackDamage = 20;
    //public Vector2 knockbackVelocity;
    public float disarmTime;
    Color newColor = new Color(1f, 0.5f, 0.5f, 1f);
    

    void Start()
    {
        currentHealth = maxHealth;
    }

    void Update() 
    {
        DefineEnemyType();
    }

#region PlayerInteraction

    private void OnTriggerEnter2D(Collider2D collision) 
    {
        if (collision.gameObject.CompareTag("RangedAttack"))
        {
            TakeDamage(playerController.bowDamage);
            
        }
    }
    public void TakeDamage(int damage)
    {
        currentHealth -= damage;
        PlaySound(clipEnemyHurt);
        animator.SetTrigger("GetHurt");
        StartCoroutine(EnemyHurt());
        if (currentHealth <=0)
        {
            Die();
        }
    }
    public IEnumerator EnemyHurt()
    {    
        sp.color = newColor;
        yield return new WaitForSeconds(disarmTime);
        sp.color = Color.white;
    }
    public void EnemyHurtPush(bool directionRight)
    {
        if(directionRight == true)
        {
            rb.MovePosition(new Vector2(rb.position.x + 0.50f, rb.position.y));
        }else
        {
            rb.MovePosition(new Vector2(rb.position.x - 0.50f, rb.position.y));
        }
    }
    void Die()
    {
        animator.SetBool("IsDead", true);
        gameObject.GetComponent<BoxCollider2D>().enabled = false;
        Destroy(gameObject,1);
    }
#endregion

#region EnemyMovement

    public enum EnemyType
    {
        Stand,
        Patrol,
        Boss
    }
    
    [Space]
    [Header("EnemyMovement")]
    
    public EnemyType Type;
    public float patrolTime;

    public void DefineEnemyType()
    {
        switch (Type)
        {
            case EnemyType.Stand:
                rb.velocity = new Vector2(0,0);
            break;

            case EnemyType.Patrol:
                if(patrolTime < 5)
                {
                    rb.velocity = new Vector2(-1, rb.velocity.y);
                    patrolTime += Time.deltaTime;
                }
                if(patrolTime > 5)
                {
                    rb.velocity = new Vector2(1, rb.velocity.y);
                    patrolTime += Time.deltaTime;
                }
                if(patrolTime >10)
                {
                    patrolTime = 0;
                }

            break;

            case EnemyType.Boss:
                
            break;

            default:

            break;
        }
    }

#endregion

#region EnemyAudio
    [Space]
    [Header("Enemy Audio")]

    public AudioClip clipEnemyHurt;

    public void PlaySound(AudioClip clip)
    {
        audioSource.clip = clip;
        audioSource.Play();
    }


#endregion
}
