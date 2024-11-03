using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class TextInputHandler : MonoBehaviour
{
    public TMP_InputField keyinput; // Input field for the user's typed text
    public Button submitButton; // Button to submit the input
    public NPC npc; // Reference to the NPC script

    void Start()
    {
        // Add listener to the submit button
        submitButton.onClick.AddListener(OnSubmit);
    }

    private void OnSubmit()
    {
        // Get the input message from the user
        string userMessage = keyinput.text;

        if (!string.IsNullOrEmpty(userMessage))
        {
            // Send message to the NPC for processing
            ProcessInput(userMessage);
        }
        else
        {
            Debug.LogError("User input is empty!");
        }

        // Clear input field after submitting
        keyinput.text = "";
    }

    private void ProcessInput(string userMessage)
    {
        // Check if NPC is assigned and send the input to NPC for processing
        if (npc != null)
        {
            // Pass the user message to the NPC script to handle input
            npc.HandleUserInput(userMessage); 
        }
        else
        {
            Debug.LogError("NPC reference is missing!");
        }
    }
}
