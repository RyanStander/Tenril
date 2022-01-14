using TMPro;
using UnityEngine;
using UnityEngine.UI;
/// <summary>
/// Holds the data for dropping stacks
/// </summary>
public class DropStackDisplay : MonoBehaviour
{
    private int amountToDrop = 1;
    private int amountThatCanBeDropped = 1;
    private Item itemToDrop;
    [SerializeField] private GameObject dropStackObject;
    [SerializeField] private Image itemImage;
    [SerializeField] private TMP_Text currentAmountToDropText;

    private void OnEnable()
    {
        EventManager.currentManager.Subscribe(EventType.InitiateDropStack, OnInitiateDropStack);
    }

    private void OnDisable()
    {
        EventManager.currentManager.Unsubscribe(EventType.InitiateDropStack, OnInitiateDropStack);
    }

    public void OnInitiateDropStack(EventData eventData)
    {
        if (eventData is InitiateDropStack dropStack)
        {
            amountToDrop = 1;
            amountThatCanBeDropped = dropStack.amountThatCanBeDropped;
            itemToDrop = dropStack.item;

            dropStackObject.SetActive(true);
            itemImage.sprite = itemToDrop.itemIcon;
            currentAmountToDropText.text = "1";
        }
        else
        {
            Debug.LogWarning("Event was not of the correct type (InitiateDropStack) cannot proceed");
        }
        
        
    }

    public void ConfirmDrop()
    {
        EventManager.currentManager.AddEvent(new CompleteDropStack(itemToDrop, amountToDrop));
        dropStackObject.SetActive(false);
    }

    public void CancelDrop()
    {
        dropStackObject.SetActive(false);
    }

    public void IncreaseDropAmount()
    {
        amountToDrop++;
        if (amountToDrop > amountThatCanBeDropped)
            amountToDrop = amountThatCanBeDropped;
        UpdateDropAmountText();
    }

    public void DecreaseDropAmount()
    {
        amountToDrop--;
        if (amountToDrop < 1)
            amountToDrop = 1;
        UpdateDropAmountText();
    }

    private void UpdateDropAmountText()
    {
        currentAmountToDropText.text = amountToDrop.ToString();
    }
}
