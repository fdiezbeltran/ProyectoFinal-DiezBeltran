using UnityEngine;
using UnityEngine.Events;

public class FinalBossTrigger : MonoBehaviour
{
    [SerializeField] private UnityEvent finalBossTrigger;

    void OnTriggerEnter2D(Collider2D col) 
    {
        if(col.CompareTag("Player"))
        {   
            Debug.Log("se esta invokando");
            finalBossTrigger.Invoke();
        }
    }
}
