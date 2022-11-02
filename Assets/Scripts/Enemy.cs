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
    public Vector2 leftMove;
    public Vector2 rightMove;
    Color newColor = new Color(1f, 0.5f, 0.5f, 1f);

    void Start()
    {
        currentHealth = maxHealth;
                rb.velocity = new Vector2(-1, 0);

    }

    void Update() 
    {
    }

    private void OnTriggerEnter2D(Collider2D collision) 
    {
        if (collision.gameObject.CompareTag("RangedAttack"))
        {
            TakeDamage(playerController.attackDamage);
            
        }
    }

    public void TakeDamage(int damage)
    {
        currentHealth -= damage;

        animator.SetTrigger("isHurted");
        StartCoroutine(HurtKnockback());


        if (currentHealth <=0)
        {
            Die();
        }
    }

    public IEnumerator HurtKnockback()
    {    
        sp.color = newColor;
        var dir = playerController.swordPoint.position - transform.position;
        rb.velocity = dir.normalized * knockbackVelocity;
        yield return new WaitForSeconds(disarmTime);
        sp.color = Color.white;
        //rb.velocity = Vector3.zero;
    }

    void Die()
    {
        animator.SetBool("IsDead", true);

        GetComponent<Collider2D>().enabled = false;
        
        DestroyGameObject();

    }

    void DestroyGameObject()
    {
        Destroy(gameObject);
    }
    //Causar danio

}
