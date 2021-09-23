using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class InteractableUI : MonoBehaviour
{
    [Tooltip("Text displayed when near the item")]
    public TextMeshProUGUI interactableText;

    [Tooltip("Text displayed when picking up the item")]
    public TextMeshProUGUI itemText;
    [Tooltip("Image of the item displayed after pick up")]
    public RawImage itemImage;
}
