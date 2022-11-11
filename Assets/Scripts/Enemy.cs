using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Enemy : MonoBehaviour
{
    public Animator animator;
    public Rigidbody2D rb;
    public SpriteRenderer sp;
    public PlayerController playerController;
    

    public int maxHealth = 100;
    int currentHealth;
    public int attackDamage = 20;
    public Vector2 knockbackVelocity;
    public float disarmTime;
    Color newColor = new Color(1f, 0.5f, 0.5f, 1f);

    void Start()
    {
        currentHealth = maxHealth;
        //rb.velocity = new Vector2(-1, 0);
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

        animator.SetTrigger("GetHurt");
        StartCoroutine(HurtKnockback());
        if (currentHealth <=0)
        {
            Die();
        }
    }
    public IEnumerator HurtKnockback()
    {    
        sp.color = newColor;
        //var dir = playerController.swordPoint.position - transform.position;
        //rb.velocity = dir.normalized * knockbackVelocity;
        yield return new WaitForSeconds(disarmTime);
        sp.color = Color.white;
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
        Chase
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

            case EnemyType.Chase:
                Debug.Log("deberia perseguirte");
            break;

            default:

            break;
        }
    }

#endregion
}
