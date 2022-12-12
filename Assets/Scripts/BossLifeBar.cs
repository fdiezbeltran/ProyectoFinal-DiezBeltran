using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class BossLifeBar : MonoBehaviour
{
    [Space]
    [Header("Barra de vida Jefe")]
    public GameObject finalBoss;
    public Image bossLifeBar;
    public float bossCurrentHealth;

    // Update is called once per frame
    void Update()
    {
        bossCurrentHealth = finalBoss.GetComponent<Enemy>().currentHealth;
        bossLifeBar.fillAmount =  bossCurrentHealth / 1000;
    }
}
