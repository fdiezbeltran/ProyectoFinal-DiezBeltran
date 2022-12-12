using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FinalBossController : MonoBehaviour
   
{
    public Rigidbody2D rb;
    public Transform bossGroundCheck;
    public LayerMask groundLayer;


    public int stateBoss;
    public float fightTime;
    public float jumpTime;
    public bool bossGrounded;
    public float bossJumpPower;
    public bool rightDirection = false;
    public bool rightPosition = false;

    //state 1 intro
    //state 2 jump
    //state 3 fireball
    //state 4 rocks

    void Start() 
    {
        stateBoss = 1;
    }

    void Update() 
    {
        fightTime += Time.deltaTime;
        HandleBossStates();


        if(fightTime > 3)
        {
            stateBoss = 2;
        }
        if(fightTime > 15)
        {
            stateBoss = 3;
        }

        jumpTime += Time.deltaTime;
        if(bossGrounded)
        {
            jumpTime = 0;
        }
    }

    void FixedUpdate()
    {
        BossGroundCheck();
    }

    void HandleBossStates()
    {
        switch (stateBoss)
        {
            case 1:
                    rb.velocity = new Vector2(0, rb.velocity.y);
                    //animacion grito
                    //audio grito
            break;

            case 2:
                    if(!rightDirection)
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
                       rb.velocity = new Vector2(6, rb.velocity.y);
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
            
            case 3:
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
                    }

            break;

            case 4:

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
        if(col.gameObject.CompareTag("RightLimit"))
        {
            rightPosition = true;
        }else
        {
            rightPosition = false;
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
