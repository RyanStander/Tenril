using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class DialogueReceiver : MonoBehaviour
{
    [SerializeField] private TMP_Text npcNameText;
    [SerializeField] private TMP_Text npcDialogueWindowText;

    private void OnEnable()
    {
        EventManager.currentManager.Subscribe(EventType.SendDialogueSentence, OnSendDialogueSentenceReceived);
    }

    private void OnDisable()
    {
        EventManager.currentManager.Unsubscribe(EventType.SendDialogueSentence, OnSendDialogueSentenceReceived);
    }

    private void OnSendDialogueSentenceReceived(EventData eventData)
    {
        if (eventData is SendDialogueSentence sendDialogueSentence)
        {
            npcNameText.text = sendDialogueSentence.npcName;
            npcDialogueWindowText.text= sendDialogueSentence.sentence;
        }
        else
        {
            throw new System.Exception("Error: EventData class with EventType.SendDialogueSentence was received but is not of class SendDialogueSentence.");
        }
    }
}
