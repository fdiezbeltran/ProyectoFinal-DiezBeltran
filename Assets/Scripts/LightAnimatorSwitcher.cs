using UnityEngine;

public class LightAnimatorSwitcher : MonoBehaviour
{
    public Animator animator;
    public bool apagable; 

    void Start()
    {
        DefineLightType();
    }

    void Update() 
    {
        
    }

    void OnTriggerEnter2D(Collider2D col) 
    {
        if(apagable)
        {
            if(col.gameObject.CompareTag("Player"))
            {
                Destroy(gameObject);
            }    
        }
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
