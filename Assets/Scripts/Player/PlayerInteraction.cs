using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerInteraction : MonoBehaviour
{
    private InteractableUI interactableUI;
    public GameObject itemPopUp;
    public GameObject dialoguePopUp;
    [SerializeField] private GameObject interactableDataHolderPrefab;

    [Header("OverlapBox setup")]
    [SerializeField] private Vector3 overlapBoxOffset;
    [SerializeField] private float overlapBoxDistance = 1f;
    [SerializeField] private Vector3 boxSize=new Vector3(1,1,1);
    [SerializeField] private LayerMask targetLayers;
    [SerializeField] private bool showGizmo;

    private InputHandler inputHandler;

    private int currentlySelectedInteractableIndex = 0;

    private void Awake()
    {
        inputHandler = GetComponent<InputHandler>();


        interactableUI = FindObjectOfType<InteractableUI>();
    }

    internal void CheckForInteractableObject()
    {
        Collider[] hitColliders = Physics.OverlapBox(transform.forward * overlapBoxDistance + transform.position +overlapBoxOffset, boxSize,Quaternion.identity,targetLayers);
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
                Interactable interactableObject = hitColliders[i].GetComponent<Interactable>();

                //if there is an interactable object
                if (interactableObject != null)
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

                   

                    //if interact button is pressed while the option is available
                    if (inputHandler.interactInput)
                    {
                        //call the interaction
                        hitColliders[currentlySelectedInteractableIndex].GetComponent<Interactable>().Interact(GetComponent<PlayerManager>());

                    }
                }
            }

            //go to next collider
            i++;
        }

        //if there are no coliders
        if (0 == hitColliders.Length)
        {
            //hide the interactable ui and reset

            if (itemPopUp != null && inputHandler.interactInput)
            {
                itemPopUp.SetActive(false);
            }
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
