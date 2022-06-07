using UnityEngine;

/// <summary>
/// Assigned to the buttons for continuing dialogue in different options. When a button or something else triggers a dialogue it will load up on the display from this function
/// </summary>
public class DialogueTrigger : MonoBehaviour
{
    public DialogueData dialogueData;

    public void TriggerDialogue()
    {
        EventManager.currentManager.AddEvent(new SendDialogueData(dialogueData));
    }
}