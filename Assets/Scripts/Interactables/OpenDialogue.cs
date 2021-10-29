using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class OpenDialogue : Interactable
{
    [SerializeField] private GameObject npcCamera;
    public override void Interact(PlayerManager playerManager)
    {
        base.Interact(playerManager);

        HandleDialogueInitiation(playerManager);
    }


    private void HandleDialogueInitiation(PlayerManager playerManager)
    {
        //change camera
        EventManager.currentManager.AddEvent(new SwapToNPCCamera(npcCamera));

        EventManager.currentManager.AddEvent(new InitiateDialogue());

        PlayerInteraction playerInteraction;

        playerInteraction = playerManager.GetComponent<PlayerInteraction>();

        //Enable the game object
        playerInteraction.dialoguePopUp.SetActive(true);

    }
}