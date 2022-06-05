using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DialogueTrigger : MonoBehaviour
{
    public DialogueData dialogueData;
    public Animator[] currentlySetAnimator;
    [SerializeField] private AudioSource[] npcAudioSources;

    public void TriggerDialogue()
    {
        EventManager.currentManager.AddEvent(new SendDialogueData(dialogueData,currentlySetAnimator,npcAudioSources));
    }
}
