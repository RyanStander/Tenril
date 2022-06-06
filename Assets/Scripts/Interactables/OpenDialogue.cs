using UnityEngine;
using UnityEngine.Localization;

public class OpenDialogue : Interactable
{
    [SerializeField] private GameObject npcCamera;
    [Tooltip("This should be synced up with string keys, each new animator is a different character to be animated")]
    [SerializeField] private Animator[] npcAnimators;
    [Tooltip("This should be synced up with string keys, each new animator is a different character to be animated")]
    [SerializeField] private AudioSource[] npcAudioSources;

    [Tooltip("Where to load the dialogue from")]
    [SerializeField] private LocalizedStringTable localizedStringTable;
    [Tooltip("The dialogue that is given at the start")]
    [SerializeField] private DialogueData initialDialogueData;
    public override void Interact(PlayerManager playerManager)
    {
        base.Interact(playerManager);

        HandleDialogueInitiation(playerManager);
    }


    private void HandleDialogueInitiation(PlayerManager playerManager)
    {
        //change camera
        EventManager.currentManager.AddEvent(new SwapToNpcCamera(npcCamera));
        
        EventManager.currentManager.AddEvent(new SendDialogueNpcInfo(npcAnimators,npcAudioSources));

        EventManager.currentManager.AddEvent(new SendStartingStringTableForDialogue(localizedStringTable));

        EventManager.currentManager.AddEvent(new InitiateDialogue());

        EventManager.currentManager.AddEvent(new SendDialogueData(initialDialogueData));
    }
}