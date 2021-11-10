using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CloseDialogueButton : MonoBehaviour
{
    public void CloseDialogueWindow()
    {
        EventManager.currentManager.AddEvent(new CeaseDialogue());
    }
}
