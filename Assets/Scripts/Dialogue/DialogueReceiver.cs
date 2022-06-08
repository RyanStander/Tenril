using TMPro;
using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// Receive dialogue info and display it to the user through ui
/// </summary>
public class DialogueReceiver : MonoBehaviour
{
    [SerializeField] private TMP_Text npcNameText;
    [SerializeField] private TMP_Text npcDialogueWindowText;
    [SerializeField] private GameObject optionButtonPrefab;
    [SerializeField] private GameObject leaveDialogueButtonPrefab;
    [SerializeField] private GameObject optionHolder;

    private void Start()
    {
        EventManager.currentManager.Subscribe(EventType.SendDialogueSentence, OnSendDialogueSentenceReceived);
        EventManager.currentManager.Subscribe(EventType.SendDialogueOptions, OnSendDialogueOptionsReceived);
    }

    private void OnSendDialogueSentenceReceived(EventData eventData)
    {
        if (eventData is SendDialogueSentence sendDialogueSentence)
        {
            //removes all previous items
            foreach (Transform child in optionHolder.transform)
            {
                Destroy(child.gameObject);
            }

            npcNameText.text = sendDialogueSentence.npcName;
            npcDialogueWindowText.text= sendDialogueSentence.sentence;
        }
        else
        {
            throw new System.Exception("Error: EventData class with EventType.SendDialogueSentence was received but is not of class SendDialogueSentence.");
        }
    }

    private void OnSendDialogueOptionsReceived(EventData eventData)
    {
        if (eventData is SendDialogueOptions sendDialogueOptions)
        {
            //removes all previous items
            foreach (Transform child in optionHolder.transform)
            {
                Destroy(child.gameObject);
            }

            //create a button to exit the menu
            Instantiate(leaveDialogueButtonPrefab, optionHolder.transform);

            if (sendDialogueOptions.options == null) return;
            
            for (var i = 0; i < sendDialogueOptions.options.Count; i++)
            {
                //create option button
                var createdOptionPrefab = Instantiate(optionButtonPrefab, optionHolder.transform);

                //get trigger script
                var dialogueTrigger = createdOptionPrefab.GetComponent<DialogueTrigger>();
                //set the option's values
                if (dialogueTrigger != null)
                {
                    dialogueTrigger.dialogueData = sendDialogueOptions.nextDialogues[i];
                }
                else
                    Debug.Log("DialogueTrigger script was not found on the object, please make sure it is on");

                //get trigger button
                var triggerButton = createdOptionPrefab.GetComponent<Button>();
                //assign button action
                if (triggerButton != null)
                    triggerButton.onClick.AddListener(dialogueTrigger.TriggerDialogue);
                else
                    Debug.Log("Button component was not found on the object, please make sure it is on");

                //get text object
                var optionText = createdOptionPrefab.GetComponentInChildren<TMP_Text>();
                optionText.text = sendDialogueOptions.options[i];
            }
        }
        else
        {
            throw new System.Exception("Error: EventData class with EventType.SendDialogueOptions was received but is not of class SendDialogueOptions.");
        }
    }

    public void ShowNextSentence()
    {
        EventManager.currentManager.AddEvent(new ShowNextSentence());
    }
}
