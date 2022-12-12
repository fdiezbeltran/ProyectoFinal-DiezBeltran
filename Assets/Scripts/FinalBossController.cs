using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FinalBossController : MonoBehaviour
   
{
    public Rigidbody2D rb;


    public int stateBoss;
    public float fightTime;

    [Space]
    [Header("State 1")]
    public Transform bossGroundCheck;
    public LayerMask groundLayer;
    public float jumpTime;
    public bool bossGrounded;
    public float bossJumpPower;
    public bool rightDirection = false;

    [Space]
    [Header("State 2")]
    public Transform bossShootPoint;
    public GameObject fireballPrefab;
    public bool rightPosition = false;
    public float fireballCooldown;
    public float fireballRate = 1;
    float fireballVelocity = 15f;

    [Space]
    [Header("State 3")]
    public Transform coronaSprite;
    Vector3 direction;
    public Transform[] escombrosPositions = new Transform[10];
    public GameObject escombroPrefab;
    public float speed;
    public bool centerPosition;
    public float escombroCooldown;
    public float escombroRate = 1;

    [Space]
    [Header("State 4")]
    public bool leftPosition = false;

    
    //

    //state 1 intro
    //state 2 jump
    //state 3 fireball
    //state 4 rocks

    void Start() 
    {
        stateBoss = 0;
    }

    void Update() 
    {
        fightTime += Time.deltaTime;


        if(fightTime > 3)
        {
            stateBoss = 1;
        }
        if(fightTime > 15)
        {
            stateBoss = 2;
        }
        if(fightTime > 30)
        {
            stateBoss = 3;
        }
        if(fightTime > 45)
        {
            stateBoss = 1;
        }
        if(fightTime > 60)
        {
            stateBoss = 4;
        }
        if(fightTime > 70)
        {
            fightTime = 3;
        }

        bossJumpPower = Random.Range(15,25);
        jumpTime += Time.deltaTime;
        if(bossGrounded)
        {
            jumpTime = 0;
        }

        direction = coronaSprite.position - transform.position;
    }

    void FixedUpdate()
    {
        HandleBossStates();
        BossGroundCheck();
    }

    void HandleBossStates()
    {
        switch (stateBoss)
        {
            case 0:
                    rb.velocity = new Vector2(0, rb.velocity.y);
                    //animacion grito
                    //audio grito
            break;

            case 1:
                    leftPosition = false;
                    rightPosition = false;
                    centerPosition = false;

                    if(!rightDirection)
                    {
                        rb.velocity = new Vector2(Random.Range(-4, -8), rb.velocity.y);
                        if(jumpTime < 0.5f)
                        {
                            rb.velocity = new Vector2(rb.velocity.x, bossJumpPower);
                        }
                        if (rb.velocity.y > 0f)
                        {
                            rb.velocity = new Vector2(rb.velocity.x, rb.velocity.y * 0.7f);
                        }
                    }else
                    {
                       rb.velocity = new Vector2(Random.Range(4, 8), rb.velocity.y);
                        if(jumpTime < 0.5f)
                        {
                            rb.velocity = new Vector2(rb.velocity.x, bossJumpPower);
                        }
                        if (rb.velocity.y > 0f)
                        {
                            rb.velocity = new Vector2(rb.velocity.x, rb.velocity.y * 0.7f);
                        } 
                    }
                    
                    
            break;
            
            case 2:
                    centerPosition = false;
                    if(!rightPosition)
                    {
                        rb.velocity = new Vector2(6, rb.velocity.y);
                        if(jumpTime < 0.5f)
                        {
                            rb.velocity = new Vector2(rb.velocity.x, bossJumpPower);
                        }
                        if (rb.velocity.y > 0f)
                        {
                            rb.velocity = new Vector2(rb.velocity.x, rb.velocity.y * 0.7f);
                        } 
                    }else
                    {
                        rb.velocity = new Vector2(0, rb.velocity.y);
                        if(jumpTime < 0.5f)
                        {
                            rb.velocity = new Vector2(rb.velocity.x, 5);
                        }
                        if (rb.velocity.y > 0f)
                        {
                            rb.velocity = new Vector2(rb.velocity.x, rb.velocity.y * 0.7f);
                        } 
                        if(fireballCooldown > fireballRate)
                        {
                            GameObject fireball = Instantiate(fireballPrefab, bossShootPoint.position, Quaternion.identity);
                            fireball.GetComponent<Rigidbody2D>().velocity = new Vector2(-fireballVelocity, 0.0f);
                            
                            fireballCooldown = 0;
                        }

                        fireballCooldown += Time.deltaTime;
                    }

            break;

            case 3:
                    rightPosition = false;
                    if(fightTime < 33)
                    {
                        rb.MovePosition(transform.position + direction.normalized * speed * Time.fixedDeltaTime);
                    }
                    if(jumpTime < 0.5f)
                        {
                            rb.velocity = new Vector2(rb.velocity.x, bossJumpPower);
                        }
                        if (rb.velocity.y > 0f)
                        {
                            rb.velocity = new Vector2(rb.velocity.x, rb.velocity.y * 0.7f);
                        }
                    
                    if(centerPosition && fightTime > 33)
                    {
                        //grita
                        if(escombroCooldown > escombroRate)
                        {
                            GameObject fireball = Instantiate(escombroPrefab, escombrosPositions[Random.Range(0,9)].position, Quaternion.identity);
                                                        
                            escombroCooldown = 0;
                        }
                        escombroCooldown += Time.deltaTime;
                    }
            break;

            case 4:
                    if(!leftPosition)
                    {
                        rb.velocity = new Vector2(-6, rb.velocity.y);
                        if(jumpTime < 0.5f)
                        {
                            rb.velocity = new Vector2(rb.velocity.x, bossJumpPower);
                        }
                        if (rb.velocity.y > 0f)
                        {
                            rb.velocity = new Vector2(rb.velocity.x, rb.velocity.y * 0.7f);
                        } 
                    }else
                    {   
                        if(jumpTime < 0.5f)
                        {
                            rb.velocity = new Vector2(rb.velocity.x, 10);
                        }
                        if (rb.velocity.y > 0f)
                        {
                            rb.velocity = new Vector2(rb.velocity.x, rb.velocity.y * 0.7f);
                        } 

                        if(escombroCooldown > escombroRate)
                        {
                            GameObject fireball = Instantiate(escombroPrefab, escombrosPositions[Random.Range(0,9)].position, Quaternion.identity);
                                                        
                            escombroCooldown = 0;
                        }
                        escombroCooldown += Time.deltaTime;
                    }

            break;

            default:

            break;
        }

    }

    void OnTriggerEnter2D(Collider2D col) 
    {
        if(col.gameObject.CompareTag("LeftLimit"))
        {
            rightDirection = true;
        }
        if(col.gameObject.CompareTag("RightLimit"))
        {
            rightDirection = false;
        }
    }
    void OnTriggerStay2D(Collider2D col) 
    {
        if(col.gameObject.CompareTag("LeftLimit"))
        {
            leftPosition = true;
        }
        if(col.gameObject.CompareTag("RightLimit"))
        {
            rightPosition = true;
        }
        if(col.gameObject.CompareTag("Corona"))
        {
            centerPosition = true;
        }
    }

    void BossGroundCheck()
    {
        RaycastHit2D bossRaycastGround = Physics2D.Raycast(bossGroundCheck.position, Vector2.down, 0.01f, groundLayer);

        if (bossRaycastGround)
        {
            bossGrounded = true;
        }
        if (!bossRaycastGround)
        {
            bossGrounded = false;
        }
    }




}
