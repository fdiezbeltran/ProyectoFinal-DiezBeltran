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
    //public GameObject[] choices;
    //private Animator layoutAnimator;
    //private TextMeshProUGUI[] choicesText;
    private Story currentStory;
    
    public bool dialogueIsPlaying { get; private set; }
    private bool canContinueToNextLine = false;
    private Coroutine displayLineCoroutine;

    private static DialogueManager instance;

    private const string NAME_TAG = "name";
    private const string PORTRAIT_TAG = "portrait";
    //private const string LAYOUT_TAG = "layout";

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

        //layoutAnimator = dialoguePanel.GetComponent<Animator>();

        /*choicesText = new TextMeshProUGUI[choices.Length];
        int index = 0;
        foreach (GameObject choice in choices) 
        {
            choicesText[index] = choice.GetComponentInChildren<TextMeshProUGUI>();
            index++;
        }*/
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

    public void EnterDialogueMode(TextAsset inkJSON)
    {
        currentStory = new Story(inkJSON.text);
        dialogueIsPlaying = true;
        dialoguePanel.SetActive(true);

        nameText.text = "???";
        portraitAnimator.Play("default");
        //layoutAnimator.Play("character");

        ContinueStory();
    }

    private void ExitDialogueMode()
    {
        dialogueIsPlaying = false;
        dialoguePanel.SetActive(false);
        dialogueText.text = "";
    }

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

    private IEnumerator DisplayLine(string line)
    {
        dialogueText.text = line;
        dialogueText.maxVisibleCharacters = 0;
        canContinueToNextLine = false;
        continueIcon.SetActive(false);
        //HideChoices();

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
        //DisplayChoices();
    }

    private void HandleTags(List<string> currentTags)
    {
        foreach (string tag in currentTags)
        {
            string[] splitTag = tag.Split(':');
            /*if (splitTag.Length !=2)
            {
                Debug.LogError("Tag could not be appropriately parsed: " + tag);
            }*/
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
                //case LAYOUT_TAG:
                    //layoutAnimator.Play(tagValue);
                    //break;
                default:
                    //Debug.LogWarning("Tag came in but is not currently being handled: " + tag);
                    break;
            }
        }
    }

    /*private void DisplayChoices() 
    {
        List<Choice> currentChoices = currentStory.currentChoices;

        if (currentChoices.Count > choices.Length)
        {
            Debug.LogError("Mas opciones de las que la UI soporta. Number of choices given: " 
                + currentChoices.Count);
        }

        int index = 0;
        foreach(Choice choice in currentChoices) 
        {
            choices[index].gameObject.SetActive(true);
            choicesText[index].text = choice.text;
            index++;
        }
        for (int i = index; i < choices.Length; i++) 
        {
            choices[i].gameObject.SetActive(false);
        }

        StartCoroutine(SelectFirstChoice());
    }

    private void HideChoices()
    {
        foreach (GameObject choiceButton in choices)
        {
            choiceButton.SetActive(false);
        }
    }


    private IEnumerator SelectFirstChoice() 
    {
        EventSystem.current.SetSelectedGameObject(null);
        yield return new WaitForEndOfFrame();
        EventSystem.current.SetSelectedGameObject(choices[0].gameObject);
    }

    public void MakeChoice(int choiceIndex)
    {
        if (canContinueToNextLine)
        {
        currentStory.ChooseChoiceIndex(choiceIndex);
        InputUI.GetInstance().RegisterSubmitPressed();
        
        ContinueStory();

        }
    }*/
}

