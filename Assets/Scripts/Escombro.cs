using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Escombro : MonoBehaviour
{
    public Rigidbody2D rb;
    public int attackDamage = 25;
    

    void Update() 
    {
        
    }


    private void OnTriggerEnter2D(Collider2D collision) 
    {
        if (collision.gameObject.CompareTag("Player"))
        {
            rb.velocity = new Vector2(0, 0);
            Destroy(gameObject, 0.1f);
        }
        if (collision.gameObject.CompareTag("Level"))
        {
            rb.velocity = new Vector2(0, 0);
            Destroy(gameObject, 0.1f);
        }
    }
}