using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class InteractableUI : MonoBehaviour
{
    [Tooltip("Where available interactable pop ups are displayed")]
    public GameObject interactionPopUpsContent;

    [Tooltip("Text displayed when picking up the item")]
    public TextMeshProUGUI itemText;
    [Tooltip("Image of the item displayed after pick up")]
    public RawImage itemImage;
}
