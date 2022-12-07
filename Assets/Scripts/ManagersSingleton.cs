using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ManagersSingleton : MonoBehaviour
{
    public static ManagersSingleton instance;

    void Awake()
    {
        if(ManagersSingleton.instance == null)
        {
            ManagersSingleton.instance = this;
            DontDestroyOnLoad(gameObject);
        }else
        {
            Destroy(gameObject);
        }
    }
    
    void Update()
    {
        
    }
}
