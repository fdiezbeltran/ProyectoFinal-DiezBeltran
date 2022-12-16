using UnityEngine;

public class DialogueTrigger : MonoBehaviour
{      
    public PlayerController playerController;
    public GameObject dialogueSign;

    public TextAsset inkJSON;
    public bool dialogueRange;
    
    private void Update() 
    {
        if (dialogueRange && !DialogueManager.GetInstance().dialogueIsPlaying)
        {
            dialogueSign.SetActive(true);
            if (playerController.interactPressed && dialogueRange)
            {
                DialogueManager.GetInstance().EnterDialogueMode(inkJSON);
            }
        }
        else
        {
            dialogueSign.SetActive(false);
        }
    }

    private void OnTriggerEnter2D(Collider2D collision) 
    {
        if (collision.gameObject.CompareTag("Player"))
        {
            playerController.dialogueRange = true;
            dialogueRange = true;
        }
    }

    private void OnTriggerExit2D(Collider2D collision) 
    {
        if (collision.gameObject.CompareTag("Player"))
        {
            playerController.dialogueRange = false;
            dialogueRange = false;
        }
    }
}