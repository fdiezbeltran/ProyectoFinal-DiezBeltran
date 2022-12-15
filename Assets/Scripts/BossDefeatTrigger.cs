using UnityEngine;
using UnityEngine.Events;


public class BossDefeatTrigger : MonoBehaviour
{
    [SerializeField] private UnityEvent defeatBossTrigger;

    public Enemy enemy;
    public float bossHealth;


    private void Update() 
    {
        bossHealth = enemy.currentHealth;
        
        if(bossHealth <= 0)
        {
            defeatBossTrigger.Invoke();
        }
    }
}
