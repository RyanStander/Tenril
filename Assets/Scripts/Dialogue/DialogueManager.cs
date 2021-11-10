using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.Localization;
using UnityEngine.Localization.Tables;

public class DialogueManager : MonoBehaviour
{
    private StringTable currentStringTable;

    //data fetched from dialogue option
    private Queue<string> sentences;
    private string currentNPCName;
    //the list of options the player can choose at the end of the dialogue
    private List<string> currentOptions;
    //the dialoge datas loaded depending on the chosen dialogue
    private List<DialogueData> followingDialogues;

    private void OnEnable()
    {
        EventManager.currentManager.Subscribe(EventType.SendDialogueData, OnSendDialogueDataReceived);
        EventManager.currentManager.Subscribe(EventType.SendStartingStringTableForDialogue, OnSendStartingStringTableForDialogueReceived);
    }

    private void OnDisable()
    {
        EventManager.currentManager.Unsubscribe(EventType.SendDialogueData, OnSendDialogueDataReceived);
        EventManager.currentManager.Unsubscribe(EventType.SendStartingStringTableForDialogue, OnSendStartingStringTableForDialogueReceived);
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
            currentNPCName = sendDialogueData.dialogueData.npcName;

            currentOptions = sendDialogueData.dialogueData.options.ToList();
            followingDialogues = sendDialogueData.dialogueData.nextDialogue.ToList();

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

    private void OnSendStartingStringTableForDialogueReceived(EventData eventData)
    {
        if (eventData is SendStartingStringTableForDialogue sendStartingStringTableForDialogue)
        {
            currentStringTable = sendStartingStringTableForDialogue.localizedStringTable.GetTable();
        }
        else
        {
            throw new System.Exception("Error: EventData class with EventType.SendStartingStringTableForDialogue was received but is not of class SendStartingStringTableForDialogue.");
        }
    }

    public void DisplayNextSentence()
    {
        //if no more sentences, end dialogue
        if (sentences.Count==0)
        {
            ShowOptions();
            return;
        }

        //Get next sentence
        string sentence = sentences.Dequeue();

        EventManager.currentManager.AddEvent(new SendDialogueSentence(currentNPCName,sentence));
    }

    private void ShowOptions()
    {
        Debug.Log("convo end");

        EventManager.currentManager.AddEvent(new SendDialogueOptions(currentOptions, followingDialogues));
    }
}
