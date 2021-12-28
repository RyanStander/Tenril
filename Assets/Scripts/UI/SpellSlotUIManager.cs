
using UnityEngine;
using UnityEngine.UI;

public class SpellSlotUIManager : MonoBehaviour
{
    [SerializeField] private PlayerInventory playerInventory;

    [SerializeField] private Image[] spellSlotImages = new Image[8];
    [SerializeField] private Image[] spellSlotKeybindImages = new Image[8];
    [SerializeField] private Image spellcastingModeKeybindImage;

    private void Start()
    {
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

        spellcastingModeKeybindImage.preserveAspect = true;
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
        for (int i = 0; i < spellSlotImages.Length; i++)
        {
            spellSlotKeybindImages[i].sprite = spellslotKeybindSprites[i];
        }

        spellcastingModeKeybindImage.sprite = spellcastingModeSprite;
    }
}
