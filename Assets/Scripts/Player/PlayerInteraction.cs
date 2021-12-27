using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Used to check for interactable objects and allowing player to interact with them
/// </summary>
public class PlayerInteraction : MonoBehaviour
{
    private InteractableUI interactableUI;
    public GameObject dialoguePopUp;
    [SerializeField] private GameObject interactableDataHolderPrefab;

    [Header("OverlapBox setup")]
    [SerializeField] private Vector3 overlapBoxOffset;
    [SerializeField] private float overlapBoxDistance = 1f;
    [SerializeField] private Vector3 boxSize=new Vector3(1,1,1);
    [SerializeField] private LayerMask targetLayers;
    [SerializeField] private bool showGizmo;

    private InputHandler inputHandler;

    private List<Interactable> interactables=new List<Interactable>();
    private int currentlySelectedInteractableIndex = 0;

    private void Awake()
    {
        inputHandler = GetComponent<InputHandler>();


        interactableUI = FindObjectOfType<InteractableUI>();
    }

    internal void CheckForInteractableObject()
    {
        if (inputHandler.alternateInteraction)
            currentlySelectedInteractableIndex++;

        FindInteractables();

        List<Interactable> temporaryInteractables = new List<Interactable>();
        temporaryInteractables.AddRange(interactables);

        //Reset the selected interactable if the top item is
        if (currentlySelectedInteractableIndex >= interactables.Count)
            currentlySelectedInteractableIndex = 0;

        List<Interactable> interacablesToRemove= new List<Interactable>();

        //Start from chosen index val
        for (int i = currentlySelectedInteractableIndex; i < temporaryInteractables.Count; i++)
        {
            //if the selected index is too large, exit out of the function
            if (currentlySelectedInteractableIndex<= temporaryInteractables.Count)
            {
                DisplayInteractableOption(temporaryInteractables[i]);

                interacablesToRemove.Add(temporaryInteractables[i]);
            }
            else
            {
                Debug.Log("interactable index is too small for for loop, please help.");
                break;
            }
        }

        foreach (Interactable interactable in interacablesToRemove)
        {
            if (temporaryInteractables.Contains(interactable))
                temporaryInteractables.Remove(interactable);
        }
        foreach (Interactable interactable in temporaryInteractables)
        {
            DisplayInteractableOption(interactable);
        }

        //Interact with the selected object
        if (inputHandler.interactInput&&interactables.Count>0)
        {
            interactables[currentlySelectedInteractableIndex].Interact(GetComponent<PlayerManager>());
        }

        //Empty list
        interactables = new List<Interactable>();
    }  

    private void DisplayInteractableOption(Interactable interactableObject)
    {
        GameObject createdGameObject = Instantiate(interactableDataHolderPrefab, interactableUI.interactionPopUpsContent.transform);
        if (createdGameObject.TryGetComponent(out InteractableDataHolder interactableDataHolder))
        {
            interactableDataHolder.interactableNameText.text = interactableObject.interactableText;

            //if the interactable is an item pickup
            if (interactableObject is ItemPickup itemPickup)
            {
                interactableDataHolder.interactableIconImage.sprite = itemPickup.item.itemIcon;
                interactableDataHolder.interactableNameText.text = itemPickup.item.itemName;
            }
        }
        else
        {
            Debug.Log("Couldnt find interactableDataHolder on the created prefab for interaction");
        }
    }

    private void FindInteractables()
    {
        Collider[] hitColliders = Physics.OverlapBox(transform.forward * overlapBoxDistance + transform.position + overlapBoxOffset, boxSize, Quaternion.identity, targetLayers);
        int i = 0;

        //destroy all previously displayed objects
        foreach (RectTransform child in interactableUI.interactionPopUpsContent.GetComponent<RectTransform>())
        {
            Destroy(child.gameObject);
        }

        //Check when there is a new collider coming into contact with the box
        while (i < hitColliders.Length)
        {
            //check if it has the tag of an interactable
            if (hitColliders[i].tag == "Interactable")
            {
                if (hitColliders[i].TryGetComponent(out Interactable interactable))
                {
                    interactables.Add(interactable);
                }
            }

            //go to next collider
            i++;
        }
    }
    private void OnDrawGizmos()
    {
        if (showGizmo)
        {
            Gizmos.color = Color.grey;
            //Use the same vars you use to draw your Overlap SPhere to draw your Wire Sphere.
            Gizmos.DrawWireCube(transform.forward * overlapBoxDistance + transform.position + overlapBoxOffset, boxSize);
        }
    }
}
