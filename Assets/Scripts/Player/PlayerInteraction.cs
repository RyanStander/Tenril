using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;

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

    private PlayerManager playerManager;
    private InputHandler inputHandler;

    private List<Interactable> interactables=new List<Interactable>();
    private List<GameObject> createdInteractableObjects = new List<GameObject>();
    private int currentlySelectedInteractableIndex = 0;

    //Information of interactables:
    //We want to keep info on an interactable in the event of requiring data from it at a later point
    private HarvestableResource currentInteractableInUse;
    private void OnEnable()
    {
        EventManager.currentManager.Subscribe(EventType.PlayerKeybindsUpdates,OnPlayerKeybindsUpdates);
        EventManager.currentManager.Subscribe(EventType.PlayerChangedInputDevice, OnPlayerChangedInputDevice);
    }

    private void OnDisable()
    {
        EventManager.currentManager.Unsubscribe(EventType.PlayerKeybindsUpdates, OnPlayerKeybindsUpdates);
        EventManager.currentManager.Unsubscribe(EventType.PlayerChangedInputDevice, OnPlayerChangedInputDevice);
    }

    private void Awake()
    {
        inputHandler = GetComponent<InputHandler>();
        playerManager = GetComponent<PlayerManager>();

        interactableUI = FindObjectOfType<InteractableUI>();
    }

    private void Start()
    {
        EventManager.currentManager.AddEvent(new PlayerKeybindsUpdate());
    }

    internal void CheckForInteractableObject()
    {
        createdInteractableObjects = new List<GameObject>();

        if (inputHandler.alternateInteraction)
            currentlySelectedInteractableIndex++;

        FindInteractables();

        //Reset the selected interactable if the top item is
        if (currentlySelectedInteractableIndex >= interactables.Count)
            currentlySelectedInteractableIndex = 0;

        foreach (Interactable interactable in interactables)
        {
            DisplayInteractableOption(interactable);
        }

        if (interactables.Count > 0)
        {
            //interactableUI.selectedItemOutline.gameObject.SetActive(true);
            //Vector3 position = createdInteractableObjects[currentlySelectedInteractableIndex].transform.position;
            //interactableUI.selectedItemOutline.transform.position = position;
            interactableUI.selectedItemOutline.gameObject.SetActive(true);
            StartCoroutine(AdjustTransInTheEndOfFrame(interactableUI.selectedItemOutline));
        }
        else
        {
            interactableUI.selectedItemOutline.gameObject.SetActive(false);
        }

        //Interact with the selected object
        if (inputHandler.interactInput&&interactables.Count>0)
        {
            interactables[currentlySelectedInteractableIndex].Interact(GetComponent<PlayerManager>());

            if (interactables[currentlySelectedInteractableIndex] is HarvestableResource harvestableResource)
            {
                currentInteractableInUse = harvestableResource;
            }
        }

        //Empty list
        interactables = new List<Interactable>();
    }  

    private void DisplayInteractableOption(Interactable interactableObject)
    {
        GameObject createdGameObject = Instantiate(interactableDataHolderPrefab, interactableUI.interactionPopUpsContent.transform);
        //Add the created interacatables to a lst for further use
        createdInteractableObjects.Add(createdGameObject);
        if (createdGameObject.TryGetComponent(out InteractableDataHolder interactableDataHolder))
        {
            interactableDataHolder.interactableNameText.text = interactableObject.interactableText;

            //if the interactable is an item pickup
            if (interactableObject is ItemPickup itemPickup)
            {
                interactableDataHolder.interactableIconImage.sprite = itemPickup.item.itemIcon;
                interactableDataHolder.interactableNameText.text = itemPickup.item.itemName+" x " + itemPickup.amountOfItem;
            }
            else if (interactableObject is HarvestableResource harvestableResource)
            {
                interactableDataHolder.interactableIconImage.sprite = harvestableResource.displayIcon;
                interactableDataHolder.interactableNameText.text = "Harvest "+ harvestableResource.interactableText;
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

    private void DisplayKeybindForInteract()
    {
        string bindingPath= CharacterUtilityManager.GetBindingPath(inputHandler.GetInputActions().CharacterControls.Interact, inputHandler.activeInputDevice);

        interactableUI.keybindToPress.sprite = CharacterUtilityManager.FindKeybindIcon(inputHandler.deviceDisplayConfigurator, inputHandler.activeInputDevice, bindingPath);
    }

    private IEnumerator AdjustTransInTheEndOfFrame(Transform obj)
    {
        yield return new WaitForEndOfFrame();
        Vector3 position = createdInteractableObjects[currentlySelectedInteractableIndex].transform.position;
        obj.position = position;
        obj.gameObject.SetActive(true);
    }
    private void OnDrawGizmos()
    {
        if (showGizmo)
        {
            Gizmos.color = Color.red;
            //Use the same vars you use to draw your Overlap SPhere to draw your Wire Sphere.
            Gizmos.DrawWireCube(transform.forward * overlapBoxDistance + transform.position + overlapBoxOffset, boxSize);
        }
    }

    private void OnPlayerKeybindsUpdates(EventData eventData)
    {
        if (eventData is PlayerKeybindsUpdate)
        {
            DisplayKeybindForInteract();
        }
        else
        {
            Debug.LogWarning("The event of PlayerKeybindsUpdates was not matching of event type PlayerKeybindsUpdates");
        }
    }

    private void OnPlayerChangedInputDevice(EventData eventData)
    {
        if (eventData is PlayerChangedInputDevice)
        {
            DisplayKeybindForInteract();
        }
        else
        {
            Debug.LogWarning("The event of PlayerChangedInputDevice was not matching of event type PlayerChangedInputDevice");
        }
    }

    /// <summary>
    /// Called by animation events
    /// </summary>
    private void HarvestItem()
    {
        if (currentInteractableInUse!=null)
        {
            //If there are no more resources to harvest
            if (!currentInteractableInUse.ObtainItemsFromHarvest())
            {
                GetComponent<PlayerAnimatorManager>().animator.SetBool("isHarvestingResource", false);
                playerManager.HideDisplayObect(true);
            }

        }
        else
        {
            GetComponent<PlayerAnimatorManager>().animator.SetBool("isHarvestingResource",false);
            playerManager.HideDisplayObect(true);
            Debug.LogWarning("No harvestable object was found");
        }

    }
}

/*
 * Harvestable Resources:
 *  -Player interacts with object
 *      -Usual stuff
 *  -Starts playing animation for a set amount of time
 *  -When reaching a point of the animation, reward player the item
 *      -Use animation event to obtain item from harvestableResource
 *  -Play a sound at part of the animation
 *  -When the duration ends, exit the animation
 */