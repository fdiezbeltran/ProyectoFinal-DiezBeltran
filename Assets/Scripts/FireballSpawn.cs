using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FireballSpawn : MonoBehaviour
{
    public GameObject fireballPrefab;
    public Transform center;

    public float fireballCooldown;
    public float fireballRate = 3;
    float fireballVelocity = 10f;


    void Update()
    {
        if(fireballCooldown > fireballRate)
        {
            GameObject fireball = Instantiate(fireballPrefab, center.position, Quaternion.identity);
            fireball.GetComponent<Rigidbody2D>().velocity = new Vector2(-fireballVelocity, 0.0f);
            
            fireballCooldown = 0;
        }

        fireballCooldown += Time.deltaTime;
    }
}
