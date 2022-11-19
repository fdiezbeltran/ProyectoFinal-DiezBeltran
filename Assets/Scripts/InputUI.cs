using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class InputUI : MonoBehaviour
{
    private bool submitPressed = false;
    public DialogueManager dialogueManager;
    
    private static InputUI instance;

    private void Awake()
    {
        if (instance != null)
        {
            Debug.LogError("Hay mas de un InputUI en la escena");
        }
        
        instance = this;
    }

    public static InputUI GetInstance() 
    {
        return instance;
    }

    public void Submit(InputAction.CallbackContext context)
    {
        if (context.performed && dialogueManager.dialogueIsPlaying)
        {
            submitPressed = true;
        }
        else if (context.canceled)
        {
            submitPressed = false;
        }
    }

    public bool GetSubmitPressed() 
    {
        bool result = submitPressed;
        submitPressed = false;
        return result;
    }

    public void RegisterSubmitPressed() 
    {
        submitPressed = false;
    }
}
