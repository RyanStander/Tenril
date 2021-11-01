using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DialogueManager : MonoBehaviour
{
    private Queue<string> sentences;
    private string currentNPCName;

    private void OnEnable()
    {
        EventManager.currentManager.Subscribe(EventType.SendDialogueData, OnSendDialogueDataReceived);
    }

    private void OnDisable()
    {
        EventManager.currentManager.Unsubscribe(EventType.SendDialogueData, OnSendDialogueDataReceived);
    }

    // Start is called before the first frame update
    private void Start()
    {
        sentences = new Queue<string>();
    }

    private void OnSendDialogueDataReceived(EventData eventData)
    {
        if (eventData is SendDialogueData sendDialogueData)
        {
            currentNPCName = sendDialogueData.dialogue.name;

            //remove all previous sentences from the queue
            sentences.Clear();

            //gather all sentences from the received dialogue and place them in the queue
            foreach(string sentence in sendDialogueData.dialogue.sentences)
            {
                sentences.Enqueue(sentence);
            }

            DisplayNextSentence();
        }
        else
        {
            throw new System.Exception("Error: EventData class with EventType.SendDialogueData was received but is not of class SendDialogueData.");
        }
    }

    public void DisplayNextSentence()
    {
        //if no more sentences, end dialogue
        if (sentences.Count==0)
        {
            EndDialogue();
            return;
        }

        //Get next sentence
        string sentence = sentences.Dequeue();

        EventManager.currentManager.AddEvent(new SendDialogueSentence(currentNPCName,sentence));
    }

    private void EndDialogue()
    {
        Debug.Log("convo end");
    }
}
