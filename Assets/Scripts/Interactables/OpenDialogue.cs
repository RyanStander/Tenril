using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.Localization;
using UnityEngine.UI;

public class OpenDialogue : Interactable
{
    [SerializeField] private GameObject npcCamera;

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
        EventManager.currentManager.AddEvent(new SwapToNPCCamera(npcCamera));

        EventManager.currentManager.AddEvent(new SendStartingStringTableForDialogue(localizedStringTable));

        EventManager.currentManager.AddEvent(new InitiateDialogue());

        EventManager.currentManager.AddEvent(new SendDialogueData(initialDialogueData));

        PlayerInteraction playerInteraction;

        playerInteraction = playerManager.GetComponent<PlayerInteraction>();

        //Enable the game object
        playerInteraction.dialoguePopUp.SetActive(true);

    }
}