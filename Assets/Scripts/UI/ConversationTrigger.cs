using System.Collections.Generic;
using UnityEngine;

public class ConversationTrigger2D : MonoBehaviour
{
    [Header("Dialogue")]

    [SerializeField] private List<DialogueLine> dialogueLines = new List<DialogueLine>();


    [Header("Trigger Once")]
    [SerializeField] private bool triggerOnlyOnce = true;

    private bool hasTriggered = false;

    private void OnTriggerEnter2D(Collider2D other)
    {
        Debug.Log("Something entered trigger: " + other.name);

        if (hasTriggered && triggerOnlyOnce)
        {
            Debug.Log("Already triggered once.");
            return;
        }

        if (other.CompareTag("Player"))
        {
            Debug.Log("Player entered trigger.");

            if (ConversationUIManager.Instance != null &&
                !ConversationUIManager.Instance.IsConversationActive())
            {
                Debug.Log("Starting conversation.");
                ConversationUIManager.Instance.StartConversation(dialogueLines);
                hasTriggered = true;
            }
            else
            {
                Debug.Log("UI Manager missing or conversation already active.");
            }
        }
        else
        {
            Debug.Log("Entered object is not tagged Player.");
        }
    }
}