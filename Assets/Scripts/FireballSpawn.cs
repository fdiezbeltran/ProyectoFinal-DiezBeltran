using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FireballSpawn : MonoBehaviour
{
    public GameObject fireballPrefab;
    public Transform center;

    public float fireballCooldown;
    public float fireballRate = 2;
    public float fireballVelocity = 15;


    void Update()
    {
        fireballCooldown = Time.time + 1f / fireballRate;
        if(Time.time >= fireballCooldown)
        {
            GameObject fireball = Instantiate(fireballPrefab, center.position, Quaternion.identity);
            fireball.GetComponent<Rigidbody2D>().velocity = new Vector2(fireballVelocity, 0.0f);
        }
    }
}
