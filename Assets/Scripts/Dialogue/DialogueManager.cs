using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Localization;
using UnityEngine.Localization.Tables;

public class DialogueManager : MonoBehaviour
{
    [SerializeField] private LocalizedStringTable dialogueStringTable;
    private StringTable currentStringTable;

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

        currentStringTable = dialogueStringTable.GetTable();
    }

    private void OnSendDialogueDataReceived(EventData eventData)
    {
        if (eventData is SendDialogueData sendDialogueData)
        {
            currentNPCName = sendDialogueData.dialogueData.npcName;

            //remove all previous sentences from the queue
            sentences.Clear();

            //gather all sentences from the received dialogue and place them in the queue
            foreach(string stringKey in sendDialogueData.dialogueData.stringKeys)
            {
                string sentence = currentStringTable[stringKey].LocalizedValue;
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
