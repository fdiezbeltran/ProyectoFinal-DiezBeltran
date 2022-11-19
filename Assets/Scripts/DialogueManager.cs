using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.EventSystems;
using TMPro;
using Ink.Runtime;


public class DialogueManager : MonoBehaviour
{
    public float typingSpeed = 0.04f;
    public GameObject dialoguePanel;
    public GameObject continueIcon;
    public TextMeshProUGUI dialogueText;
    public TextMeshProUGUI nameText;
    public Animator portraitAnimator;
    private Story currentStory;
    
    public bool dialogueIsPlaying { get; private set; }
    private bool canContinueToNextLine = false;
    private Coroutine displayLineCoroutine;

    private static DialogueManager instance;

    private const string NAME_TAG = "name";
    private const string PORTRAIT_TAG = "portrait";

    private void Awake() 
    {
        if (instance != null)
        {
            Debug.Log("Hay mas de un Dialogue Manager en esta escena");
        }
        instance = this;
    }

    public static DialogueManager GetInstance()
    {
        return instance;
    }

    private void Start() 
    {
        dialogueIsPlaying = false;
        dialoguePanel.SetActive(false);
    }

    public void Update()
    {
        if (!dialogueIsPlaying)
        {
            return;
        }    
        
        if (canContinueToNextLine
            && currentStory.currentChoices.Count == 0
            && InputUI.GetInstance().GetSubmitPressed())
        {
            ContinueStory();
        }
    }

    //Empieza el dialogo
    public void EnterDialogueMode(TextAsset inkJSON)
    {
        currentStory = new Story(inkJSON.text);
        dialogueIsPlaying = true;
        dialoguePanel.SetActive(true);

        nameText.text = "???";
        portraitAnimator.Play("default");

        ContinueStory();
    }
    //Termina el dialogo
    private void ExitDialogueMode()
    {
        dialogueIsPlaying = false;
        dialoguePanel.SetActive(false);
        dialogueText.text = "";
    }
    //Pasa a la siguiente frase
    private void ContinueStory()
    {
        if (currentStory.canContinue)
        {
            if(displayLineCoroutine != null)
            {
                StopCoroutine(displayLineCoroutine);
            }
            displayLineCoroutine = StartCoroutine(DisplayLine(currentStory.Continue()));
            
            HandleTags(currentStory.currentTags);
        }
        else
        {
            ExitDialogueMode();
        }
    }
    
    //Muestra la frase letra por letra
    private IEnumerator DisplayLine(string line)
    {
        dialogueText.text = line;
        dialogueText.maxVisibleCharacters = 0;
        canContinueToNextLine = false;
        continueIcon.SetActive(false);
        bool isAddingRichTexTag = false;

        foreach (char letter in line.ToCharArray())
        {
            if (InputUI.GetInstance().GetSubmitPressed())
            {
                dialogueText.maxVisibleCharacters = line.Length;
                break;
            }
            if (letter == '<' || isAddingRichTexTag)
            {
                isAddingRichTexTag = true;

                if (letter == '>')
                {
                    isAddingRichTexTag = false;
                }
            }
            else
            {
                dialogueText.maxVisibleCharacters++;
                yield return new WaitForSeconds(typingSpeed);
            }
        }
        canContinueToNextLine = true;
        continueIcon.SetActive(true);
    }
    
    //Tags para definir nombre y retrato
    private void HandleTags(List<string> currentTags)
    {
        foreach (string tag in currentTags)
        {
            string[] splitTag = tag.Split(':');
            string tagKey = splitTag[0].Trim();
            string tagValue = splitTag[1].Trim();

            switch (tagKey)
            {
                case NAME_TAG:
                    nameText.text = tagValue;
                    break;
                case PORTRAIT_TAG:
                    portraitAnimator.Play(tagValue);
                    break;
                default:
                    break;
            }
        }
    }
}

