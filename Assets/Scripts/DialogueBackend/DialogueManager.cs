//Adapted from: https://pastebin.com/DgyxWJ5T Accessed: November 26, 2025
//manager

using System;
using System.Collections;
using System.Threading;
using TMPro;
using Unity.Cinemachine;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.EventSystems;
using UnityEngine.InputSystem;
using UnityEngine.UI;
 
public class DialogueManager : MonoBehaviour
{
    public static DialogueManager Instance { get; private set; }
    
    // UI references
    [Header("UI references")]
    public GameObject DialogueParent; // Main container for dialogue UI
    public TextMeshProUGUI DialogTitleText, DialogBodyText; // Text components for title and body
    public GameObject responseButtonPrefab; // Prefab for generating response buttons
    public Transform responseButtonContainer; // Container to hold response buttons
    
    [Header("Other references")]
    [SerializeField] private PlayerInput playerInput;
    [SerializeField] private EventSystem eventSystem;

    //Type write effect
    [SerializeField] private float timeBetweenLettersTyped = 0.01f;
    private bool isTyping = false;

    private const string HTML_ALPHA = "<color=#00000000>";
    
    private void Awake()
    {
        // Singleton pattern to ensure only one instance of DialogueManager
        if (Instance == null)
        {
            Instance = this;
        }
        else
        {
            Destroy(gameObject);
        }
 
        // Initially hide the dialogue UI
        HideDialogue();
    }
 
    // Starts the dialogue with given title and dialogue node
    public void StartDialogue(string title, DialogueNode node)
    {
        // Display the dialogue UI
        ShowDialogue();
 
        // Set dialogue title and body text
        DialogTitleText.text = title;
        
        StartTypingEffect(node.dialogueText);
 
        // Remove any existing response buttons
        foreach (Transform child in responseButtonContainer)
        {
            Destroy(child.gameObject);
        }
 
        // Create and setup response buttons based on current dialogue node
        foreach (DialogueResponse response in node.responses)
        {
            GameObject buttonObj = Instantiate(responseButtonPrefab, responseButtonContainer);
            buttonObj.GetComponentInChildren<TextMeshProUGUI>().text = response.responseText;
            
            //Selects a button so we can navigate
            eventSystem.SetSelectedGameObject(buttonObj);
 
            // Setup button to trigger SelectResponse when clicked
            buttonObj.GetComponent<Button>().onClick.AddListener(() => ResponseButtonClicked(response, title, node.dialogueText));
        }
        
        playerInput.SwitchCurrentActionMap("UI"); //Switch player input to UI, will stop them moving as a result
    }

    private void ResponseButtonClicked(DialogueResponse response, string title, string currentDialogueText)
    {
        if (isTyping)
        {
            FinishTyping(currentDialogueText);
            isTyping = false;
            return;
        }
        
        SelectResponse(response, title);
    }
 
    // Handles response selection and triggers next dialogue node
    public void SelectResponse(DialogueResponse response, string title)
    {
        
        // Check if there's a follow-up node
        if (!response.nextNode.IsLastNode())
        {
            StartDialogue(title, response.nextNode); // Start next dialogue
        }
        else
        {
            FinishDialogue();
        }
        
        //Invoke an event to occur on dialogue end
        response.nextNode.onDialogue?.Invoke(); 
    }
 
    // Hide the dialogue UI
    public void HideDialogue()
    {
        DialogueParent.SetActive(false);
    }

    private void FinishDialogue()
    {
        // If no follow-up node, end the dialogue
        HideDialogue();
            
        eventSystem.SetSelectedGameObject(null); //Get rid of any select game object!
        playerInput.SwitchCurrentActionMap("Player"); //Switch player input back to the player
        
        
    }

    private void StartTypingEffect(string finishedText)
    {
        StartCoroutine(TypingEffect(finishedText));
    }
    private IEnumerator TypingEffect(string finishedText)
    {
        isTyping = true;
        DialogBodyText.text = "";
    
        string originalText = ParseText(finishedText);
        int charIndex = -1;
        int rawIndex = -1;
        float timeToWait = timeBetweenLettersTyped;
        bool parsingSpeed = false;
        string speedParsing = "";
    
        while (charIndex < originalText.Length - 1)
        {
            //Tracks the unparsed code
            rawIndex++;
            if (rawIndex >= finishedText.Length) { break; }
        
            // Check the new char for parsing purposes
            char newChar = finishedText[rawIndex];
        
            // End speed parsing with default speed
            if (parsingSpeed && newChar == '/')
            {
                timeToWait = timeBetweenLettersTyped;
                rawIndex++; // Skip the '>'
                parsingSpeed = false;
                speedParsing = "";
                continue;
            }
            
            // End speed parsing with custom speed
            if (parsingSpeed && newChar == '>')
            {
                timeToWait = float.Parse(speedParsing);
                parsingSpeed = false;
                speedParsing = "";
                continue;
            }
            
            if (parsingSpeed)
            {
                speedParsing += newChar;
                continue;
            }
        
            // Start speed parsing
            if (newChar == '<')
            {
                parsingSpeed = true;
                continue;
            }
        
            // This is an actual character to display
            charIndex++;
            
            //Display screen
            DialogBodyText.text = originalText;
            string displayedText = DialogBodyText.text.Insert(charIndex + 1, HTML_ALPHA);
            DialogBodyText.text = displayedText;
        
            yield return new WaitForSeconds(timeToWait);
        }
        
        DialogBodyText.text = originalText;
        isTyping = false;
    }

    private void FinishTyping(string finishedText)
    {
        StopAllCoroutines();
        DialogBodyText.text = ParseText(finishedText);
    }
    
    private string ParseText(string finishedText)
    {
        string accumulator = "";
        bool ignoring = false;
        int charIndex = 0;
    
        while (charIndex < finishedText.Length)
        {
            char newChar = finishedText[charIndex];
            if (newChar == '<')
            { ignoring = true; }
            else if (newChar == '>')
            { ignoring = false; }
            else if (!ignoring)
            { accumulator += newChar; }
            charIndex++;
        }
    
        return accumulator;
    }
 
    // Show the dialogue UI
    private void ShowDialogue()
    {
        DialogueParent.SetActive(true);
    }
 
    // Check if dialogue is currently active
    public bool IsDialogueActive()
    {
        return DialogueParent.activeSelf;
    }
}
