using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.Localization.Tables;

/// <summary>
/// the main point for dialogue, sends out and receives data to different objects in computed form.
/// </summary>
public class DialogueManager : MonoBehaviour
{
    private StringTable currentStringTable;

    //data fetched from dialogue option
    private Queue<string> sentences;

    private int currentDialogueIndex;
    private DialogueData currentSetDialogueData;
    private AudioSource[] currentSetNpcAudioSources;
    private Animator[] currentSetNpcAnimators;

    private string currentNPCName;

    //the list of options the player can choose at the end of the dialogue
    private List<string> currentOptions;

    //the dialogue datas loaded depending on the chosen dialogue
    private List<DialogueData> followingDialogues;

    private void OnEnable()
    {
        EventManager.currentManager.Subscribe(EventType.SendDialogueData, OnSendDialogueDataReceived);
        EventManager.currentManager.Subscribe(EventType.SendStartingStringTableForDialogue,
            OnSendStartingStringTableForDialogueReceived);
        EventManager.currentManager.Subscribe(EventType.ShowNextSentence, OnShowNextSentence);
        EventManager.currentManager.Subscribe(EventType.SendDialogueNpcInfo, OnSendDialogueNpcInfo);
    }

    private void OnDisable()
    {
        EventManager.currentManager.Unsubscribe(EventType.SendDialogueData, OnSendDialogueDataReceived);
        EventManager.currentManager.Unsubscribe(EventType.SendStartingStringTableForDialogue,
            OnSendStartingStringTableForDialogueReceived);
        EventManager.currentManager.Unsubscribe(EventType.ShowNextSentence, OnShowNextSentence);
        EventManager.currentManager.Unsubscribe(EventType.SendDialogueNpcInfo, OnSendDialogueNpcInfo);
    }

    // Start is called before the first frame update
    private void Start()
    {
        sentences = new Queue<string>();
    }

    private void DisplayNextSentence()
    {
        //if no more sentences, end dialogue
        if (sentences.Count == 0)
        {
            ShowOptions();
            return;
        }

        //Get next sentence
        var sentence = sentences.Dequeue();

        //extra changes to functionality
        PlayAudio();
        PlayAnimation();
        SwapCameraPosition();

        currentDialogueIndex++;

        EventManager.currentManager.AddEvent(new SendDialogueSentence(currentNPCName, sentence));
    }

    private void PlayAudio()
    {
        if (currentSetDialogueData.dialogueExtras[currentDialogueIndex].audioClipsToPlay.Length <= 0) return;
        
        //get the audio clip to play
        var clip = currentSetDialogueData.dialogueExtras[currentDialogueIndex]
            .audioClipsToPlay[currentDialogueIndex];
        
        //make sure it isn't null
        if (clip == null) return;
        
        //add clip and play it
        currentSetNpcAudioSources[currentDialogueIndex].clip = clip;
        currentSetNpcAudioSources[currentDialogueIndex].Play();
    }

    private void PlayAnimation()
    {
        if (currentSetDialogueData.dialogueExtras[currentDialogueIndex].animationsToPlay.Length <= 0) return;
        
        //get the audio clip to play
        var anim = currentSetDialogueData.dialogueExtras[currentDialogueIndex]
            .animationsToPlay[currentDialogueIndex];
        
        //make sure it isn't null
        if (anim == "") return;
        
        //play animation
        currentSetNpcAnimators[currentDialogueIndex].CrossFade(anim,0.2f);
    }

    private void SwapCameraPosition()
    {
        Debug.LogWarning("No camera swapping implemented");
    }

    private void ShowOptions()
    {
        EventManager.currentManager.AddEvent(new SendDialogueOptions(currentOptions, followingDialogues));
    }

    #region On Events

    private void OnSendDialogueNpcInfo(EventData eventData)
    {
        if (eventData is SendDialogueNpcInfo sendDialogueNpcInfo)
        {
            currentSetNpcAnimators = sendDialogueNpcInfo.npcAnimators;
            currentSetNpcAudioSources = sendDialogueNpcInfo.npcAudioSources;
        }
        else
        {
            throw new System.Exception(
                "Error: EventData class with EventType.SendDialogueData was received but is not of class SendDialogueData.");
        }
    }

    private void OnSendDialogueDataReceived(EventData eventData)
    {
        if (eventData is SendDialogueData sendDialogueData)
        {
            currentDialogueIndex = 0;

            currentNPCName = sendDialogueData.dialogueData.npcName;

            currentOptions = sendDialogueData.dialogueData.options.ToList();
            followingDialogues = sendDialogueData.dialogueData.nextDialogue.ToList();

            currentSetDialogueData = sendDialogueData.dialogueData;

            //remove all previous sentences from the queue
            sentences.Clear();

            //gather all sentences from the received dialogue and place them in the queue
            foreach (var stringKey in sendDialogueData.dialogueData.stringKeys)
            {
                var sentence = currentStringTable[stringKey].LocalizedValue;
                sentences.Enqueue(sentence);
            }

            DisplayNextSentence();
        }
        else
        {
            throw new System.Exception(
                "Error: EventData class with EventType.SendDialogueData was received but is not of class SendDialogueData.");
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
            throw new System.Exception(
                "Error: EventData class with EventType.SendStartingStringTableForDialogue was received but is not of class SendStartingStringTableForDialogue.");
        }
    }

    private void OnShowNextSentence(EventData eventData)
    {
        if (eventData is ShowNextSentence)
        {
            DisplayNextSentence();
        }
        else
        {
            throw new System.Exception(
                "Error: EventData class with EventType.ShowNextSentence was received but is not of class ShowNextSentence.");
        }
    }

    #endregion
}