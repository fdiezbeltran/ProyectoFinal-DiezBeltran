using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FinalBossController : MonoBehaviour
   
{
    public Rigidbody2D rb;
    public int stateBoss;
    public float fightTime;
    public float jumpCooldown;
    public float bossJumpPower;

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

        jumpCooldown += Time.deltaTime;
        if(jumpCooldown > 4)
        {
            jumpCooldown = 0;
        }
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
                    rb.velocity = new Vector2(-5, rb.velocity.y);
                    if(jumpCooldown > 2)
                    {
                        rb.velocity = new Vector2(rb.velocity.x, bossJumpPower);
                    }else
                    {
                        rb.velocity = new Vector2(rb.velocity.x, -5);
                    }
                    
            break;
            
            case 3:

            break;

            case 4:

            break;

            default:

            break;
        }

    }





}
