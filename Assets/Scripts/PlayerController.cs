using System.Collections;
using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerController : MonoBehaviour
{
    //Llamado de componentes
    public Rigidbody2D rb;
    public Animator animator;
    public SpriteRenderer sp;
    public Collider2D col;
    public AudioSource audioSource;
    
    //Inicializar valores
    void Start()
    {
        InitializeHealth();
        InitializeSpeed();
        StartCoroutine(LevelStartPosition());
    }

    //Manejar metodos
    void Update()
    {
        UpdateUIHealth();
        HandleFlip();
        HandleAttack();
        HandleBow();
        HandleShield();
        InmunityColorChange();
        StopPlayerInDialogue();
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

#region PlayerPositionStart

    public IEnumerator LevelStartPosition()
    {
        canMove = false;
        moveDirection = 1;
        animator.SetFloat("horizontalAnim", 1f);
        yield return new WaitForSeconds(0.5f);
        moveDirection = 0;
        animator.SetFloat("horizontalAnim", 0f);
        canMove = true; 
    }

#endregion

#region PlayerStats
    
    [Space]
    [Header("Player Stats")]

    //Valores del jugador que necesite ir cambiando

    public static int playerMaxHealth = 100; // Define la vida maxima del jugador
    public int currentHealth; // Vida actual del jugador
    public float movementSpeed = 8; //Velocidad a la que corre
    public float blockingSpeed = 4; //Define la velocidad de movimiento mientras bloquea
    private float bowingSpeed = 0; //Define la velocidad mientras se dispara el arco
    public int attackDamage = 25; //Define el danio que hace con la espada
    public float attackRate = 1.5f; //Define el tiempo para atacar de nuevo con la espada
    public int bowDamage = 15; //Define el danio que hace con el arco
    public float bowAttackRate = 1.5f; //Define el tiempo para atacar de nuevo con el arco

    public static int uiHealth;

    void InitializeHealth()
    {
        currentHealth = playerMaxHealth;
    }
    void InitializeSpeed()
    {
        speed = movementSpeed;
    }

    void UpdateUIHealth()
    {
        uiHealth = currentHealth;
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
    private float jumpingPower = 16;
    
    public void Move(InputAction.CallbackContext context)
    {
       if(canMove && !dialogueManager.dialogueIsPlaying)
       {
            if (context.performed)
            {
                moveDirection = context.ReadValue<Vector2>().x;
                animator.SetFloat("horizontalAnim", moveDirection);
            }
            else if (context.canceled)
            {
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
        if(!playerIsDead)
        {
            if (context.performed && isGrounded && !isBlocking  && !dialogueManager.dialogueIsPlaying)
            {
                rb.velocity = new Vector2(rb.velocity.x, jumpingPower);
            }
            if (context.performed && airTime < 0.075f && !isJumping && !isBlocking  && !dialogueManager.dialogueIsPlaying)
            {
                rb.velocity = new Vector2(rb.velocity.x, jumpingPower);
            }
            if (context.canceled && rb.velocity.y > 0f)
            {
                rb.velocity = new Vector2(rb.velocity.x, rb.velocity.y * 0.5f);
            }
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
    public Transform center;
    public LayerMask enemyLayer;
    public ParticleSystem hit;
    
    public float arrowVelocity = 15; //Define la velocidad a la que viaja la flecha
    public float swordRange = 0.6f; //Define la distancia de ataque de la espada
    public float cooldownRate = 4f; //Define un minimo de tiempo entre ataques

    float nextAttackTime = 0f;
    float nextBowTime = 0f;
    float attackGlobalCooldown = 0f;
    private bool isBlocking = false;
    private bool isBowing = false;
    private bool canMove = true;

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
        if(!playerIsDead)
        {
            if(attackPressed && !dialogueManager.dialogueIsPlaying)
            {
                if (Time.time >= nextAttackTime && Time.time >= attackGlobalCooldown)
                {
                    animator.SetTrigger("AttackSword");
                    PlaySound(clipSword);
                    Collider2D[] hitEnemies = Physics2D.OverlapCircleAll(swordPoint.position, swordRange, enemyLayer);

                    foreach (Collider2D enemy in hitEnemies)
                    {
                        enemy.GetComponent<Enemy>().TakeDamage(attackDamage);
                        hit.Play();
                        if(isFacingRight)
                        {
                            rb.MovePosition(new Vector2(rb.position.x - 0.15f, rb.position.y));
                            enemy.GetComponent<Enemy>().EnemyHurtPush(true);
                        }else                    
                        {
                            rb.MovePosition(new Vector2(rb.position.x + 0.15f, rb.position.y));
                            enemy.GetComponent<Enemy>().EnemyHurtPush(false);
                        }
                    }
                    
                    nextAttackTime = Time.time + 1f / attackRate;
                    attackGlobalCooldown = Time.time + 1f / cooldownRate;
                }
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
        if(!playerIsDead)
        {
            if (secondaryPressed && isGrounded && !dialogueManager.dialogueIsPlaying)
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
        if(!playerIsDead)
        {
            if (bowPressed && !dialogueManager.dialogueIsPlaying)
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

#region PlayerDamageAndBlock
    [Space]
    [Header("Player Damage and Block")]
    public Transform damagePoint;
    public Transform blockPoint;
    public Transform blockBackPoint;

    public Vector2 damageRange = new Vector2(0.57f, 0.97f); //Define el rango en que puede ser atacado
    public Vector2 blockRange = new Vector2(0.31f, 0.94f); //Define el rango de bloqueo con el escudo
    public Vector2 blockBackRange = new Vector2(0.31f, 0.94f); //Define el rango de bloqueo con el escudo
    public Vector2 pushbackVelocity = new Vector2(5, 10); //Define la fuerza del rechazo al ser daniado
    public float blockingTime = 0.3f; //Define el tiempo por el que no se puede mover despues de ser golpeado
    private bool damageInmune = false;
    public float inmunityTime = 3;
    Color halfAlpha = new Color(1f, 0.8f, 0.8f, 0.5f);
    private Vector3 enemyPosition;

    //Golpeado por enemigo
    public void HitByEnemy()
    {
        if (!damageInmune && !isBlocking)
        {
            Collider2D[] getHit = Physics2D.OverlapBoxAll(damagePoint.position, damageRange, 0f, enemyLayer);

            foreach (Collider2D enemy in getHit)
            {
                if(enemy.gameObject.CompareTag("Enemy"))
                {
                    TakeDamage(enemy.GetComponent<Enemy>().attackDamage);
                    enemyPosition = enemy.GetComponent<Enemy>().transform.position;
                }
                if(enemy.gameObject.CompareTag("Fireball"))
                {
                    TakeDamage(enemy.GetComponent<Fireball>().attackDamage);
                }if(enemy.gameObject.CompareTag("FuegoAzul"))
                {
                    TakeDamage(enemy.GetComponent<Fireball>().attackDamage);
                }
                if(enemy.gameObject.CompareTag("Escombro"))
                {
                    TakeDamage(enemy.GetComponent<Escombro>().attackDamage);
                }
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
        StartCoroutine(DamagePushback());
        }else if (currentHealth <= 0)
        {
            Die();
        }
    }
    //Inmunidad temporal
    public IEnumerator DamagePushback()
    {        
        Physics2D.IgnoreLayerCollision(8, 7, true);
        damageInmune = true;
        PlaySound(clipHurt);
        var dir = center.position - enemyPosition;
        rb.velocity = dir.normalized * pushbackVelocity;
        yield return new WaitForSeconds(inmunityTime);
        sp.color = Color.white;
        Physics2D.IgnoreLayerCollision(8, 7, false);
        damageInmune = false;
    } 
    void InmunityColorChange()
    {   
        if(damageInmune)
        {
            sp.color = Color.Lerp(halfAlpha, Color.white, Mathf.PingPong(Time.time * 2, 0.5f));
        }
    }
    //Bloquear con el escudo
    public void BlockAttack()
    {
        if(isBlocking)
        {
            Collider2D[] getBlock = Physics2D.OverlapBoxAll(blockPoint.position, blockRange, 0f, enemyLayer);

            foreach (Collider2D enemy in getBlock)
            {
                StartCoroutine(BlockKnockback());

                if(enemy.gameObject.CompareTag("FuegoAzul"))
                {   
                    currentHealth = currentHealth - 1;
                }
            }
        
            Collider2D[] getHitBack = Physics2D.OverlapBoxAll(blockBackPoint.position, blockBackRange, 0f, enemyLayer);

            foreach (Collider2D enemy in getHitBack)
            {
                if(!damageInmune)
                {
                    enemyPosition = enemy.GetComponent<Enemy>().transform.position;
                    TakeDamage(enemy.GetComponent<Enemy>().attackDamage);
                    secondaryPressed = false;
                }
            }
        }
    }
    //Retroceder por bloqueo
    public IEnumerator BlockKnockback()
    {
        canMove = false;
        if(isFacingRight)
        {
            moveDirection = -1;
            animator.SetFloat("horizontalAnim", -1f);
        }else                    
        {
            moveDirection = 1;
            animator.SetFloat("horizontalAnim", 1f);
        }
        yield return new WaitForSeconds(blockingTime);
        moveDirection = 0;
        animator.SetFloat("horizontalAnim", 0f);
        canMove = true; 
    }
#endregion

#region PlayerRespawn
    [Space]
    [Header("Player Respawn")]

    public Transform checkPoint;
    public bool playerIsDead = false;

    public void Die()
    {   
        if(!playerIsDead)
        {
            canMove = false;
            moveDirection = 0;
            animator.SetFloat("horizontalAnim", 0f);
            animator.SetBool("IsDead", true);
            PlaySound(clipDie);
            rb.velocity = Vector3.zero;
            playerIsDead = true;
            Invoke("Respawn", 1.5f);
        }
    }
    public void Respawn()
    {
        transform.position = checkPoint.transform.position;
        canMove = true;
        playerIsDead = false;
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
    public AudioClip clipHurt;
    

    public void PlaySound(AudioClip clip)
    {
        audioSource.clip = clip;
        audioSource.Play();
    }


#endregion

#region PlayerDialogue
    [Space]
    [Header("Player Dialogue")]
    public DialogueManager dialogueManager;

    public bool dialogueRange;
    public bool interactPressed;

    public void Interact(InputAction.CallbackContext context)
    {
        if (context.performed && !dialogueManager.dialogueIsPlaying)
        {
            interactPressed = true;
        }
        if (context.canceled)
        {
            interactPressed = false;
        }
    }

    void StopPlayerInDialogue()
    {
        if(dialogueManager.dialogueIsPlaying)
        {
            moveDirection = 0;
            animator.SetFloat("horizontalAnim", 0);
        }
    }

#endregion


//Esto es para ver radios
    void OnDrawGizmosSelected() {
        Gizmos.DrawWireCube(blockPoint.position, blockRange);
        Gizmos.DrawWireCube(blockBackPoint.position, blockBackRange);
        Gizmos.DrawWireCube(damagePoint.position, damageRange);
        Gizmos.DrawWireSphere(swordPoint.position, swordRange);
        //Gizmos.DrawWireSphere(center.position, 0.5f);
    }


}
