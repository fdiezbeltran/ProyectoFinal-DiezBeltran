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
    public AudioSource audioSource;
    
    //Inicializar valores
    void Start()
    {
        InitializeHealth();
        InitializeSpeed();
    }

    //Manejar metodos
    void Update()
    {
        HandleFlip();
        HandleAttack();
        HandleBow();
        HandleShield();
    }

    //Manejar metodos con fisicas
    void FixedUpdate() 
    {
        HandleMovement();
        SetJumpAnimator();
        GroundCheck();
        HitByEnemy();
        BlockAttack();
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
    public int attackDamage = 25; //Define el danio que hace con la espada
    public float attackRate = 1.5f; //Define el tiempo para atacar de nuevo con la espada
    public int bowDamage = 15; //Define el danio que hace con el arco
    public float bowAttackRate = 1.5f; //Define el tiempo para atacar de nuevo con el arco

    void InitializeHealth()
    {
        currentHealth = playerMaxHealth;
    }
    void InitializeSpeed()
    {
        speed = movementSpeed;
    }

#endregion

#region PlayerMovement
    [Space]
    [Header("Player Movement")]

    public Transform groundCheck1;
    public Transform groundCheck2;
    public LayerMask groundLayer;

    private float moveDirection;
    private bool isFacingRight = true;
    public float speed;
    private bool isGrounded;
    private float airTime;
    private bool isJumping;
    private float jumpingPower = 16; //Fuerza del salto
    
    public void Move(InputAction.CallbackContext context)
    {
       if(canMove) //Esto es para evitar que se mueva cuando recibe danio
       {
            if (context.performed)
            {
                moveDirection = context.ReadValue<Vector2>().x;
                animator.SetFloat("horizontalAnim", moveDirection);
            }
            else if (context.canceled)
            {
                //moveDirection = context.ReadValue<Vector2>().x;
                moveDirection = 0;
                animator.SetFloat("horizontalAnim", moveDirection);
            } 
       }else
        {
            var lerpedVelocity = Mathf.Lerp(rb.velocity.x, 0f, Time.deltaTime * 5);
            rb.velocity = new Vector2(lerpedVelocity, rb.velocity.y);
        }
    }

    void HandleMovement()
    {
        rb.velocity = new Vector2(moveDirection * speed, rb.velocity.y);
    }
    void HandleFlip()
    {
            if (!isFacingRight && moveDirection > 0f && !isBlocking && !isBowing)
            {
                Flip();
            }
            else if (isFacingRight && moveDirection < 0f && !isBlocking && !isBowing)
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
            rb.velocity = new Vector2(rb.velocity.x, jumpingPower);
        }
        if (context.performed && airTime < 0.075f && !isJumping && !isBlocking)
        {
            rb.velocity = new Vector2(rb.velocity.x, jumpingPower);
        }
        if (context.canceled && rb.velocity.y > 0f)
        {
            rb.velocity = new Vector2(rb.velocity.x, rb.velocity.y * 0.5f);
        }
    }
    void SetJumpAnimator()
    {
        animator.SetFloat("jumpVelocity", rb.velocity.y);
    }
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
    
    public float arrowVelocity = 15; //Define la velocidad a la que viaja la flecha
    public float swordRange = 0.6f; //Define la distancia de ataque de la espada
    public Vector2 blockRange = new Vector2(0.46f, 0.91f); //Define el rango de bloqueo con el escudo
    public Vector2 defenseRange = new Vector2(0.66f, 0.97f); //Define el rango en que puede ser atacado
    public float disarmTime = 0.5f; //Define el tiempo por el que no se puede mover despues de ser golpeado
    public Vector2 knockbackVelocity = new Vector2(5, 10); //Define la fuerza del rechazo al ser daniado
    public Vector2 knockbackBlockVelocity = new Vector2(3, 2); //Define la fuerza del rechazo al bloquear
    public float cooldownRate = 0.5f; //Define un minimo de tiempo entre ataques

    float nextAttackTime = 0f;
    float nextBowTime = 0f;
    float attackGlobalCooldown = 0f;
    private bool isBlocking = false;
    private bool isBowing = false;
    private bool canMove = true;
    private bool attackBlocked;

    private bool attackPressed;
    private bool secondaryPressed;
    private bool bowPressed;

    //Espada
    public void Attack(InputAction.CallbackContext context)
    {
        if (context.performed && !isBlocking)
        {
            attackPressed = true;
        }
        if (context.canceled)
        {
            attackPressed = false;
        }
    }
    void HandleAttack()
    {
        if(attackPressed)
        {
            if (Time.time >= nextAttackTime && Time.time >= attackGlobalCooldown)
            {
                animator.SetTrigger("AttackSword");
                PlaySound(clipSword);
                Collider2D[] hitEnemies = Physics2D.OverlapCircleAll(swordPoint.position, swordRange, enemyLayer);

                foreach (Collider2D enemy in hitEnemies)
                {
                    enemy.GetComponent<Enemy>().TakeDamage(attackDamage);
                }
                
                nextAttackTime = Time.time + 1f / attackRate;
                attackGlobalCooldown = Time.time + 1f / cooldownRate;
            }
        }
    }
    //Escudo
    public void Secondary(InputAction.CallbackContext context)
    {
        if (context.performed && !isBlocking)
        {
            secondaryPressed = true;
        }
        if (context.canceled)
        {
            secondaryPressed = false;
        }
    }
    void HandleShield()
    {
        if (secondaryPressed && isGrounded)
        {
            speed = blockingSpeed;
            isBlocking = true;
            animator.SetBool("IsBlocking", isBlocking);
        }
        if (!secondaryPressed && isBlocking)
        {
            speed = movementSpeed;
            isBlocking = false;
            animator.SetBool("IsBlocking", isBlocking);
        }
    }
    //Arco
    public void Bow(InputAction.CallbackContext context)
    {
        if (context.performed && !isBlocking)
        {
            bowPressed = true;
        }
        if (context.canceled)
        {
            bowPressed = false;
        }
    }
    void HandleBow()
    {
        if (bowPressed)
        {         
            animator.SetBool("AttackBow", true);
            isBowing = true;
            speed = bowingSpeed;
            if (Time.time >= nextBowTime && Time.time >= attackGlobalCooldown)
            {   
                if(isFacingRight)
                {
                    PlaySound(clipArrow);
                    Invoke("ShotArrow", 0.2f);
                    animator.SetTrigger("AttackBowT");
                    nextBowTime = Time.time + 1f / bowAttackRate;
                    attackGlobalCooldown = Time.time + 1f / cooldownRate;
                }else
                {
                    PlaySound(clipArrow);
                    Invoke("ShotArrowBack", 0.2f);
                    animator.SetTrigger("AttackBowT");
                    nextBowTime = Time.time + 1f / bowAttackRate;
                    attackGlobalCooldown = Time.time + 1f / cooldownRate;
                }
            }
        }else
        {
            animator.SetBool("AttackBow", false);
            isBowing = false;
            speed = movementSpeed;
        }
    }

    void ShotArrow()
    {
        GameObject arrow = Instantiate(arrowPrefab, center.position, Quaternion.identity);
        arrow.GetComponent<Rigidbody2D>().velocity = new Vector2(arrowVelocity, 0.0f);
        Destroy(arrow, 2f);
    }
    void ShotArrowBack()
    {
        GameObject arrow = Instantiate(arrowPrefab, center.position, Quaternion.identity);
        arrow.GetComponent<Rigidbody2D>().velocity = new Vector2(-arrowVelocity, 0.0f);
        arrow.transform.Rotate(0f, 0f, 180f);
        Destroy(arrow, 2f);
    }

#endregion

#region PlayerInteract
    [Space]
    [Header("Player Interact")]

    public Transform checkPoint;
    Color newColor = new Color(1f, 0.5f, 0.5f, 1f);
    private Vector3 enemyPosition;

    //Golpeado por enemigo
    public void HitByEnemy()
    {
        if (canMove && !attackBlocked)
        {
            Collider2D[] getHit = Physics2D.OverlapBoxAll(defensePoint.position, defenseRange, 0f, enemyLayer);

            foreach (Collider2D enemy in getHit)
            {
                TakeDamage(enemy.GetComponent<Enemy>().attackDamage);
                enemyPosition = enemy.GetComponent<Enemy>().transform.position;
            }

        }
    }
    //Descontar danio
    public void TakeDamage(int damage)
    {
        currentHealth -= damage;
        if (currentHealth > 0)
        {
        animator.SetTrigger("Hurt");
        StartCoroutine(HurtKnockback());
        }else if (currentHealth <= 0)
        {
            Die();
        }
    }
    //Limitar el movimiento y empujar para atras al player
    public IEnumerator HurtKnockback()
    {    
        isBlocking = false;
        canMove = false;
        sp.color = newColor;
        var dir = center.position - enemyPosition;
        rb.velocity = dir.normalized * knockbackVelocity;
        moveDirection = 0;
        yield return new WaitForSeconds(disarmTime);
        rb.velocity = Vector3.zero;
        animator.SetFloat("horizontalAnim", 0f);
        sp.color = Color.white;
        canMove = true;
    }
    //Bloquear con el escudo
    public void BlockAttack()
    {
        if(isBlocking)
        {
            Collider2D[] getBlock = Physics2D.OverlapBoxAll(blockPoint.position, blockRange, 0f, enemyLayer);

            foreach (Collider2D enemy in getBlock)
            {
                enemyPosition = enemy.GetComponent<Enemy>().transform.position;
                StartCoroutine(BlockKnockback());
            }
        }
    }
    //Retroceder por bloqueo
    public IEnumerator BlockKnockback()
    {
        canMove = false;
        attackBlocked = true;
        var dir = center.position - enemyPosition;
        rb.velocity = dir.normalized * knockbackBlockVelocity;
        moveDirection = 0;
        yield return new WaitForSeconds(disarmTime);
        rb.velocity = Vector3.zero;
        animator.SetFloat("horizontalAnim", 0f);
        canMove = true;
        attackBlocked = false;   
    }
    public void Die()
    {   
        canMove = false;
        moveDirection = 0;
        animator.SetFloat("horizontalAnim", 0f);
        animator.SetBool("IsDead", true);
        PlaySound(clipDie);
        rb.velocity = Vector3.zero;
        Invoke("Respawn", 1.5f);
    }
    public void Respawn()
    {
        transform.position = checkPoint.transform.position;
        canMove = true;
        animator.SetFloat("horizontalAnim", 0f);
        animator.SetBool("IsDead", false);
        currentHealth = playerMaxHealth;
    }
#endregion

#region PlayerAudio
    [Space]
    [Header("Player Audio")]

    public AudioClip clipSword;
    public AudioClip clipArrow;
    public AudioClip clipDie;

    public void PlaySound(AudioClip clip)
    {
        audioSource.clip = clip;
        audioSource.Play();
    }


#endregion

//Esto es para ver radios
    void OnDrawGizmosSelected() {
        Gizmos.DrawWireCube(blockPoint.position, blockRange);
        Gizmos.DrawWireCube(defensePoint.position, defenseRange);
        Gizmos.DrawWireSphere(swordPoint.position, swordRange);
        Gizmos.DrawWireSphere(center.position, 0.5f);
    }
}
