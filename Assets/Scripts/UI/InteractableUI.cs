using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class InteractableUI : MonoBehaviour
{
    [Tooltip("Where available interactable pop ups are displayed")]
    public GameObject interactionPopUpsContent;

    [Tooltip("This is where new items will be displayed when obtained temporarily")]
    public GameObject itemObtainedDisplayContent;
    [Tooltip("The prefabs that will be shown when player obtains an item")]
    [SerializeField] private GameObject itemPopUpPrefab;

    [Tooltip("Text displayed when picking up the item")]
    public TextMeshProUGUI itemText;
    [Tooltip("Image of the item displayed after pick up")]
    public RawImage itemImage;

    private void OnEnable()
    {
        EventManager.currentManager.Subscribe(EventType.PlayerObtainedItem, OnPlayerObtainedItem);
    }

    private void OnDisable()
    {
        EventManager.currentManager.Unsubscribe(EventType.PlayerObtainedItem, OnPlayerObtainedItem);
    }

    private void OnPlayerObtainedItem(EventData eventData)
    {
        if (eventData is PlayerObtainedItem obtainedItem)
        {
            GameObject itemPopUp = Instantiate(itemPopUpPrefab, itemObtainedDisplayContent.GetComponent<RectTransform>());

            if (itemPopUp.TryGetComponent(out ItemObtainedDisplay itemObtained))
            {
                itemObtained.itemImage.sprite = obtainedItem.itemObtained.itemIcon;
                itemObtained.itemNameText.text = obtainedItem.itemObtained.itemName;

                //TO DO, display count obtained
            }
            else
            {
                Debug.LogWarning("Couldnt find ItemObtainedDisplay, cannot enter data");
            }
        }
        else
        {
            Debug.LogWarning("The event of PlayerObtainedItem was not of type PlayerObtainedItem");
        }
    }
}
