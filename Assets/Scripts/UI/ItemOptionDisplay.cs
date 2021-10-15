using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class ItemOptionDisplay : MonoBehaviour,IPointerClickHandler
{
    [Tooltip("Text object that displays the details")]
    [SerializeField] private GameObject optionObject;
    private bool isObjectActive = false;//tells whether the text is on or off
    private GameObject availableOptions;//the tooltip object
    private InventoryOptionHolder inventoryOptionHolder;
    [SerializeField] private InventoryItemDisplay inventoryItemDisplay;

    private Transform instantiateLocation;

    private void Awake()
    {
        instantiateLocation = GameObject.Find("PlayerDisplayUI").transform;
    }

    public void OnPointerClick(PointerEventData eventData)
    {
        //if the object is active, destroy it
        if (isObjectActive)
        {
            Destroy(availableOptions);
        }

        if (optionObject != null)
        {
            //destroy previous option holders
            EventManager.currentManager.AddEvent(new DestroyInventoryOptionHolders());

            //set the object active
            isObjectActive = true;

            //get the mouse position
            Vector3 mousePos = Input.mousePosition;
            //when hovering over the object, place the text below the mouse
            availableOptions = Instantiate(optionObject, new Vector3(mousePos.x, mousePos.y + Screen.height * 0.025f, mousePos.z), optionObject.transform.rotation, instantiateLocation);
            //get the option holder
            inventoryOptionHolder = availableOptions.GetComponent<InventoryOptionHolder>();
            //display the desired objects
            switch (inventoryItemDisplay.itemType)
            {
                case "weapon":
                    DisplayWeaponButtons();
                break;
                case "quickslot":
                    DisplayQuickslotButtons();
                    break;
                default:
                    Debug.Log("no function for " + inventoryItemDisplay.itemType + " type");
                    break;
            }

            //display drop item option
            inventoryOptionHolder.dropItemButton.SetActive(true);
            inventoryOptionHolder.dropItemButton.GetComponent<Button>().onClick.AddListener(inventoryItemDisplay.DropItem);
            inventoryOptionHolder.dropItemButton.GetComponent<Button>().onClick.AddListener(OptionSelected);
        }
        else
        {
            Debug.LogWarning("There is no tooltip prefab set, please add one or this code will not function");
        }
    }

    public void OptionSelected()
    {
        if (isObjectActive)
        {
            isObjectActive = false;
            inventoryOptionHolder.UnsubscribeOptionHolderDestroyEvent();
            Destroy(availableOptions);
        }
    }

    public void DisplayWeaponButtons()
    {
        //primary wepaon
        inventoryOptionHolder.equipPrimaryWeaponButton.SetActive(true);
        inventoryOptionHolder.equipPrimaryWeaponButton.GetComponent<Button>().onClick.AddListener(inventoryItemDisplay.EquipPrimaryWeapon);
        inventoryOptionHolder.equipPrimaryWeaponButton.GetComponent<Button>().onClick.AddListener(OptionSelected);
        //secondary weapon
        inventoryOptionHolder.equipSecondaryWeaponButton.SetActive(true);
        inventoryOptionHolder.equipSecondaryWeaponButton.GetComponent<Button>().onClick.AddListener(inventoryItemDisplay.EquipSecondaryWeapon);
        inventoryOptionHolder.equipSecondaryWeaponButton.GetComponent<Button>().onClick.AddListener(OptionSelected);
    }

    public void DisplayQuickslotButtons()
    {
        //use item
        inventoryOptionHolder.useItemButton.SetActive(true);
        inventoryOptionHolder.useItemButton.GetComponent<Button>().onClick.AddListener(inventoryItemDisplay.UseItem);
        inventoryOptionHolder.useItemButton.GetComponent<Button>().onClick.AddListener(OptionSelected);
        //add quickslot item
        inventoryOptionHolder.addToQuickSlotButton.SetActive(true);
        inventoryOptionHolder.addToQuickSlotButton.GetComponent<Button>().onClick.AddListener(inventoryItemDisplay.AddQuickslotItem);
        inventoryOptionHolder.addToQuickSlotButton.GetComponent<Button>().onClick.AddListener(OptionSelected);
        //remove quickslot item
        inventoryOptionHolder.removeQuickSlotButton.SetActive(true);
        inventoryOptionHolder.removeQuickSlotButton.GetComponent<Button>().onClick.AddListener(inventoryItemDisplay.RemoveQuickslotItem);
        inventoryOptionHolder.removeQuickSlotButton.GetComponent<Button>().onClick.AddListener(OptionSelected);
    }
}

