using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class OpenDialogue : Interactable
{
    public override void Interact(PlayerManager playerManager)
    {
        base.Interact(playerManager);

        EventManager.currentManager.AddEvent(new InitiateDialogue());
    }
}
//change camera

//lock player movement

//hide player model

//open dialogue window

//let npc greet player