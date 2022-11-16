using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LightAnimatorSwitcher : MonoBehaviour
{
    public Animator animator;    

    // Start is called before the first frame update
    void Start()
    {
        DefineLightType();
    }

public enum LightType
    {
        A,
        B,
        C
    }
    
    [Space]
    [Header("Light Type")]
    
    public LightType Type;

    public void DefineLightType()
    {
        switch (Type)
        {
            case LightType.A:
                animator.SetBool("A", true);
            break;

            case LightType.B:
                animator.SetBool("B", true);
            break;

            case LightType.C:
               animator.SetBool("C", true);
            break;

            default:

            break;
        }
    }
}
