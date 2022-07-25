using System.Collections;
using System.Collections.Generic;
using Interactables;
using UnityEngine;

namespace Player
{
    /// <summary>
    /// Used to check for interactable objects and allowing player to interact with them
    /// </summary>
    public class PlayerInteraction : MonoBehaviour
    {
        #region Serialize Fields

        [SerializeField] private GameObject interactableDataHolderPrefab;

        [Header("OverlapBox setup")] [SerializeField]
        private Vector3 overlapBoxOffset;

        [SerializeField] private float overlapBoxDistance = 1f;
        [SerializeField] private Vector3 boxSize = new Vector3(1, 1, 1);
        [SerializeField] private LayerMask targetLayers;
        [SerializeField] private bool showGizmo;

        [Header("Auto-set")] [SerializeField] private InputHandler inputHandler;
        [SerializeField] private PlayerManager playerManager;
        [SerializeField] private InteractableUI interactableUi;

        #endregion

        #region Private Fields

        private List<Interactable> interactables = new List<Interactable>();
        private List<GameObject> createdInteractableObjects = new List<GameObject>();
        private int currentlySelectedInteractableIndex = 0;

        private HarvestableResource currentInteractableInUse;

        private static readonly int IsHarvestingResource = Animator.StringToHash("isHarvestingResource");

        #endregion

        #region Lifecycle

        private void OnValidate()
        {
            if (inputHandler == null)
                inputHandler = GetComponent<InputHandler>();

            if (playerManager == null)
                playerManager = GetComponent<PlayerManager>();

            if (interactableUi == null)
                interactableUi = FindObjectOfType<InteractableUI>();
        }

        private void OnEnable()
        {
            EventManager.currentManager.Subscribe(EventType.PlayerKeybindsUpdates, OnPlayerKeybindsUpdates);
            EventManager.currentManager.Subscribe(EventType.PlayerChangedInputDevice, OnPlayerChangedInputDevice);
        }

        private void OnDisable()
        {
            EventManager.currentManager.Unsubscribe(EventType.PlayerKeybindsUpdates, OnPlayerKeybindsUpdates);
            EventManager.currentManager.Unsubscribe(EventType.PlayerChangedInputDevice, OnPlayerChangedInputDevice);
        }

        private void Start()
        {
            EventManager.currentManager.AddEvent(new PlayerKeybindsUpdate());
        }

        #endregion

        #region Internal Functions

        internal void CheckForInteractableObject()
        {
            createdInteractableObjects = new List<GameObject>();

            FindInteractables();

            ChangeSelectedInteractable();

            foreach (var interactable in interactables)
                DisplayInteractableOption(interactable);

            if (interactables.Count > 0)
            {
                interactableUi.selectedItemOutline.gameObject.SetActive(true);
                StartCoroutine(AdjustTransformInTheEndOfFrame(interactableUi.selectedItemOutline));
            }
            else
                interactableUi.selectedItemOutline.gameObject.SetActive(false);

            //Interact with the selected object
            if (inputHandler.interactInput && interactables.Count > 0)
            {
                interactables[currentlySelectedInteractableIndex].Interact(playerManager);

                if (interactables[currentlySelectedInteractableIndex] is HarvestableResource harvestableResource)
                    currentInteractableInUse = harvestableResource;
            }

            //Empty list
            interactables = new List<Interactable>();
        }

        #endregion

        #region Private Functions

        private void ChangeSelectedInteractable()
        {
            if (inputHandler.alternateInteraction)
            {
                currentlySelectedInteractableIndex++;
            }

            //Reset the selected interactable if the top item is
            if (currentlySelectedInteractableIndex >= interactables.Count)
            {
                currentlySelectedInteractableIndex = 0;
            }
        }

        private void DisplayInteractableOption(Interactable interactableObject)
        {
            var createdGameObject =
                Instantiate(interactableDataHolderPrefab, interactableUi.interactionPopUpsContent.transform);

            //Add the created interactables to a lst for further use
            createdInteractableObjects.Add(createdGameObject);
            if (!createdGameObject.TryGetComponent(out InteractableDataHolder interactableDataHolder))
                return;
            interactableDataHolder.interactableNameText.text = interactableObject.interactableText;

            switch (interactableObject)
            {
                //if the interactable is an item pickup
                case ItemPickup itemPickup:
                    interactableDataHolder.interactableIconImage.sprite = itemPickup.item.itemIcon;
                    interactableDataHolder.interactableNameText.text =
                        itemPickup.item.itemName + " x " + itemPickup.amountOfItem;
                    break;
                case HarvestableResource harvestableResource:
                    interactableDataHolder.interactableIconImage.sprite = harvestableResource.displayIcon;
                    interactableDataHolder.interactableNameText.text =
                        "Harvest " + harvestableResource.interactableText;
                    break;
            }
        }

        private void FindInteractables()
        {
            var ownTransform = transform;
            var hitColliders =
                Physics.OverlapBox(ownTransform.forward * overlapBoxDistance + ownTransform.position + overlapBoxOffset,
                    boxSize,
                    Quaternion.identity, targetLayers);
            var i = 0;

            //destroy all previously displayed objects
            foreach (RectTransform child in interactableUi.interactionPopUpsContent.GetComponent<RectTransform>())
            {
                Destroy(child.gameObject);
            }

            //Check when there is a new collider coming into contact with the box
            while (i < hitColliders.Length)
            {
                //check if it has the tag of an interactable
                if (hitColliders[i].CompareTag("Interactable"))
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
            var bindingPath =
                CharacterUtilityManager.GetBindingPath(inputHandler.GetInputActions().CharacterControls.Interact,
                    inputHandler.activeInputDevice);

            interactableUi.keybindToPress.sprite =
                CharacterUtilityManager.FindKeybindIcon(inputHandler.deviceDisplayConfigurator,
                    inputHandler.activeInputDevice, bindingPath);
        }

        private IEnumerator AdjustTransformInTheEndOfFrame(Transform obj)
        {
            yield return new WaitForEndOfFrame();
            var position = createdInteractableObjects[currentlySelectedInteractableIndex].transform.position;
            obj.position = position;
            obj.gameObject.SetActive(true);
        }

        /// <summary>
        /// Called by animation events
        /// </summary>
        private void HarvestItem()
        {
            if (currentInteractableInUse != null)
            {
                //If there are no more resources to harvest
                if (currentInteractableInUse.ObtainItemsFromHarvest())
                    return;

                GetComponent<PlayerAnimatorManager>().animator.SetBool(IsHarvestingResource, false);
                playerManager.HideDisplayObject(true);
            }
            else
            {
                GetComponent<PlayerAnimatorManager>().animator.SetBool(IsHarvestingResource, false);
                playerManager.HideDisplayObject(true);
                Debug.LogWarning("No harvestable object was found");
            }
        }

        #endregion

        #region On Events

        private void OnDrawGizmos()
        {
            if (!showGizmo)
                return;
            Gizmos.color = Color.red;
            //Use the same vars you use to draw your Overlap Sphere to draw your Wire Sphere.
            var ownTransform = transform;
            Gizmos.DrawWireCube(ownTransform.forward * overlapBoxDistance + ownTransform.position + overlapBoxOffset,
                boxSize);
        }

        private void OnPlayerKeybindsUpdates(EventData eventData)
        {
            if (eventData is PlayerKeybindsUpdate)
            {
                DisplayKeybindForInteract();
            }
            else
            {
                Debug.LogWarning(
                    "The event of PlayerKeybindsUpdates was not matching of event type PlayerKeybindsUpdates");
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
                Debug.LogWarning(
                    "The event of PlayerChangedInputDevice was not matching of event type PlayerChangedInputDevice");
            }
        }

        #endregion
    }
}