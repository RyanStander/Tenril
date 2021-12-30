using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// Displays spells slots and each of their keybinds, will also manage functions of swapping to display spellcasting mode is active
/// </summary>
public class SpellSlotUIManager : MonoBehaviour
{
    private PlayerInventory playerInventory;

    [Header("Icons")]
    [Tooltip("The images that will display the spells the player has prepared")]
    [SerializeField] private Image[] spellSlotImages = new Image[8];
    [Tooltip("The keybinds that displays what button is to be pressed when casting a spell")]
    [SerializeField] private Image[] spellSlotKeybindImages = new Image[8];
    [Tooltip("The keybind that displays what button is pressed to enable spellcasting")]
    [SerializeField] private Image spellcastingModeKeybindImage;
    [Tooltip("Used for when swapping to inactive and active spellcasting modes, active will display the button inputs")]
    [SerializeField] private RectTransform spells1to4,spells5to8,spells1to4InactiveLocation, spells5to8InactiveLocation, spells1to4ActiveLocation, spells5to8ActiveLocation;
    [Tooltip("The scale of the object when active/inactive spellcasting")]
    [SerializeField] private float inactiveScale=1, activeScale=1.5f;

    private void OnEnable()
    {
        EventManager.currentManager.Subscribe(EventType.PlayerToggleSpellcastingMode, OnPlayerToggleSpellcastingMode);
    }

    private void OnDisable()
    {
        EventManager.currentManager.Unsubscribe(EventType.PlayerToggleSpellcastingMode, OnPlayerToggleSpellcastingMode);
    }

    private void Start()
    {
        //get players inventory and load spells
        if (playerInventory==null)
        {
            GameObject[] player = GameObject.FindGameObjectsWithTag("Player");
            if (player.Length!=0)
            {
                playerInventory= player[0].GetComponent<PlayerInventory>();
            }
        }
        if (playerInventory != null)
            LoadSpells();
        else
            Debug.LogWarning("Could not find PlayerInventory script in scene, make sure it exists");

        //We dont want any weird stretching on these images so we keep their aspect ratio set
        spellcastingModeKeybindImage.preserveAspect = true;
        foreach (Image image in spellSlotKeybindImages)
        {
            image.preserveAspect = true;
        }
    }

    private void LoadSpells()
    {
        if (playerInventory == null)
            return;

        //iterates through the spell slot images and assigns prepared spells from the inventory to these images
        for (int i = 0; i < spellSlotImages.Length; i++)
        {
            if (playerInventory.preparedSpells[i]!=null)
            spellSlotImages[i].sprite = playerInventory.preparedSpells[i].itemIcon;
        }
    }

    public void UpdateKeybinds(Sprite spellcastingModeSprite,Sprite[] spellslotKeybindSprites)
    {
        //Updates the keybinds to display what the current bindings are
        for (int i = 0; i < spellSlotImages.Length; i++)
        {
            spellSlotKeybindImages[i].sprite = spellslotKeybindSprites[i];
        }

        spellcastingModeKeybindImage.sprite = spellcastingModeSprite;
    }

    private void OnPlayerToggleSpellcastingMode(EventData eventData)
    {
        //modifies the ui to display spellcasting mode as active/inactive
        if (eventData is PlayerToggleSpellcastingMode playerToggleSpellcastingMode)
        {
            if (playerToggleSpellcastingMode.enteredSpellcastingMode)
            {
                spells1to4.localScale = new Vector3(activeScale, activeScale, activeScale);
                spells1to4.transform.position = spells1to4ActiveLocation.transform.position;
                
                spells5to8.localScale = new Vector3(activeScale, activeScale, activeScale);
                spells5to8.transform.position = spells5to8ActiveLocation.transform.position;
                
                foreach (Image image in spellSlotKeybindImages)
                {
                    image.gameObject.SetActive(true);
                }
                spellcastingModeKeybindImage.gameObject.SetActive(false);
            }
            else
            {
                spells1to4.localScale = new Vector3(inactiveScale, inactiveScale, inactiveScale);
                spells1to4.transform.position = spells1to4InactiveLocation.transform.position;
                
                spells5to8.localScale = new Vector3(inactiveScale, inactiveScale, inactiveScale);
                spells5to8.transform.position = spells5to8InactiveLocation.transform.position;
                
                foreach (Image image in spellSlotKeybindImages)
                {
                    image.gameObject.SetActive(false);
                }
                spellcastingModeKeybindImage.gameObject.SetActive(true);
            }
        }
    }
}
