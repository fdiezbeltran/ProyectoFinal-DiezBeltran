using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.InputSystem;

public class PlayerController : MonoBehaviour
{
    //Llamado de componentes
    public Rigidbody2D rb;
    public Animator animator;
    public SpriteRenderer sp;
    public Collider2D jumpCollider;
    
    
    
    //Inicializar valores
    void Start()
    {
        currentHealth = playerMaxHealth;
        speed = movementSpeed;
    }

    //Manejar metodos
    void Update()
    {
        HandleFlip();
        RoofCheck();
    }

    //Manejar metodos con fisicas
    void FixedUpdate() 
    {
        GroundCheck();
        animator.SetFloat("jumpVelocity", rb.velocity.y);
    }

#region PlayerStats
    
    [Space]
    [Header("Player Stats")]

    //Valores del jugador que necesite ir cambiando

    public int playerMaxHealth = 100; // Define la vida maxima del jugador
    public int currentHealth; // Vida actual del jugador
    public float movementSpeed = 8; //Velocidad a la que corre
    public float blockingSpeed = 4; //Define la velocidad de movimiento mientras bloquea
    public float bowingSpeed = 4; //Define la velocidad mientras se dispara el arco
    public int attackDamage = 25; //Define el danio que hace
    public float attackRate = 1.5f; //Define el tiempo para atacar de nuevo
      
#endregion

#region PlayerMovement
    [Space]
    [Header("Player Movement")]

    public Transform groundCheck1;
    public Transform groundCheck2;
    public LayerMask groundLayer;
    
    
    private float moveDirection;
    private bool isFacingRight = true;
    private float speed;
    private bool isGrounded;
    private float airTime;
    private bool isJumping;
    private float jumpingPower = 16; //Fuerza del salto
    private bool isBlocking = false;
    //private bool canMove = true;
    
    public void Move(InputAction.CallbackContext context)
    {
       if(canMove) //Esto es para evitar que se mueva cuando recibe danio
       {
            if (context.performed)
            {
                moveDirection = context.ReadValue<Vector2>().x;
                animator.SetFloat("horizontalAnim", moveDirection);
                rb.velocity = new Vector2(moveDirection * speed, rb.velocity.y);
            }
            else if (context.canceled)
            {
                moveDirection = context.ReadValue<Vector2>().x;
                animator.SetFloat("horizontalAnim", moveDirection);
                rb.velocity = new Vector2(moveDirection * speed, rb.velocity.y);
            } 
       }else
        {
            var lerpedVelocity = Mathf.Lerp(rb.velocity.x, 0f, Time.deltaTime * 5);
            rb.velocity = new Vector2(lerpedVelocity, rb.velocity.y);
        }
    }

    void HandleFlip()
    {
            if (!isFacingRight && moveDirection > 0f && !isBlocking)
            {
                Flip();
            }
            else if (isFacingRight && moveDirection < 0f && !isBlocking)
            {
                Flip();
            }
    }
    private void Flip()
    {
        isFacingRight = !isFacingRight;
        Vector3 localScale = transform.localScale;
        localScale.x *= -1f;
        transform.localScale = localScale;
    }    

    public void Jump(InputAction.CallbackContext context)
    {
        if (context.performed && isGrounded && !isBlocking)
        {
            Debug.Log("DEBERIAS SALTAR LA CONCHA DE TU MADRE");
            rb.velocity = new Vector2(rb.velocity.x, jumpingPower);
            
        }
        if (context.performed && airTime < 0.1f && !isJumping && !isBlocking)
        {
            rb.velocity = new Vector2(rb.velocity.x, jumpingPower);
        }
        if (context.canceled && rb.velocity.y > 0f)
        {
            rb.velocity = new Vector2(rb.velocity.x, rb.velocity.y * 0.5f);
        }
    }
    //public Vector2 groundRange;

    void GroundCheck()
    {
        RaycastHit2D raycastGround1 = Physics2D.Raycast(groundCheck1.position, Vector2.down, 0.01f, groundLayer);
        RaycastHit2D raycastGround2 = Physics2D.Raycast(groundCheck2.position, Vector2.down, 0.01f, groundLayer);
    
        animator.SetBool("IsGroundedAnim", isGrounded);

        if (raycastGround1 || raycastGround2)
        {
            isGrounded = true;
            isJumping = false;
            airTime = 0;

        }
        if (!raycastGround1 && !raycastGround2)
        {
            isGrounded = false;
            airTime += Time.deltaTime;
        }
    }
    void RoofCheck()
    {
        if(rb.velocity.y > 2f)
        {
            jumpCollider.enabled = true;
        }else
        {
            jumpCollider.enabled = false;
        }
    }

#endregion

#region PlayerCombat
    [Space]
    [Header("Player Combat")]

    public GameObject arrowPrefab;
    public Transform swordPoint;
    public Transform defensePoint;
    public Transform blockPoint;
    public Transform center;
    public LayerMask enemyLayer;
    
    public float arrowVelocity; //Define la velocidad a la que viaja la flecha
    public float swordRange; //Define la distancia de ataque de la espada
    public Vector2 blockRange; //Define el rango de bloqueo con el escudo
    public Vector2 defenseRange; //Define el rango en que puede ser atacado
    public float disarmTime; //Define el tiempo por el que no se puede mover despues de ser golpeado
    public Vector2 knockbackVelocity; //Define la fuerza del rechazo al ser daniado
    public Vector2 knockbackBlockVelocity; //Define la fuerza del rechazo al bloquear

    float nextAttackTime = 0f;
    //private bool isBlocking = false;
    private bool isBarrier = false;
    private bool canMove = true;
    private bool attackBlocked;
    private Vector3 enemyPosition;

    //Espada
    public void Attack(InputAction.CallbackContext context)
    {
        if (context.performed && !isBlocking)
        {
            if (Time.time >= nextAttackTime)
            {
                animator.SetTrigger("AttackSword");
                Collider2D[] hitEnemies = Physics2D.OverlapCircleAll(swordPoint.position, swordRange, enemyLayer);

                foreach (Collider2D enemy in hitEnemies)
                {
                    enemy.GetComponent<Enemy>().TakeDamage(attackDamage);
                }
                
                nextAttackTime = Time.time + 1f / attackRate;
            }
        }
    }
    

    //Escudo
    public void Secondary(InputAction.CallbackContext context)
    {
        if (context.performed && isGrounded)
        {
            isBlocking = true;
            animator.SetBool("IsBlocking", isBlocking);
            speed = blockingSpeed;
        }
        if (context.canceled && isBlocking)
        {
            isBlocking = false;
            animator.SetBool("IsBlocking", isBlocking);
            speed = movementSpeed;
        }
    }
    
    public IEnumerator BlockKnockback()
    {
        canMove = false;
        attackBlocked = true;
        var dir = center.position - enemyPosition;
        rb.velocity = dir.normalized * knockbackBlockVelocity;
        moveDirection = 0;
        yield return new WaitForSeconds(disarmTime);
        rb.velocity = Vector3.zero;
        canMove = true;
        attackBlocked = false;   
    }

    //Arco
    public void Bow(InputAction.CallbackContext context)
    {
        if (context.performed)
        {            
            if (Time.time >= nextAttackTime)
            {   
                if(isFacingRight)
                {
                    animator.SetTrigger("AttackBow");
                    GameObject arrow = Instantiate(arrowPrefab, center.position, Quaternion.identity);
                    arrow.GetComponent<Rigidbody2D>().velocity = new Vector2(arrowVelocity, 0.0f);
                    Destroy(arrow, 2f);
                    nextAttackTime = Time.time + 1f / attackRate;
                    speed = bowingSpeed;
                }else
                {
                    animator.SetTrigger("AttackBow");
                    GameObject arrow = Instantiate(arrowPrefab, center.position, Quaternion.identity);
                    arrow.GetComponent<Rigidbody2D>().velocity = new Vector2(-arrowVelocity, 0.0f);
                    arrow.transform.Rotate(0f, 0f, 180f);
                    Destroy(arrow, 2f);
                    nextAttackTime = Time.time + 1f / attackRate;
                    speed = bowingSpeed;
                }
            }
        }else
        {
            speed = movementSpeed;
        }
    }
    


#endregion
}
