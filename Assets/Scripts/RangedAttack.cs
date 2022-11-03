using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RangedAttack : MonoBehaviour
{
    private void OnTriggerEnter2D(Collider2D collision) 
    {
        if (collision.gameObject.CompareTag("Enemy"))
        {
            Destroy(gameObject);
            
        }
        if (collision.gameObject.CompareTag("Level"))
        {
            Destroy(gameObject);
            
        }
    }
    
}
