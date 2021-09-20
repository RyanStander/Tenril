using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class SpellSlotUIManager : MonoBehaviour
{
    [SerializeField] private PlayerInventory playerInventory;

    [SerializeField] private Image[] spellSlotImages = new Image[8];

    private void Start()
    {
        if (playerInventory==null)
        {
            playerInventory=GameObject.FindGameObjectWithTag("Player").GetComponent<PlayerInventory>();
        }
        LoadSpells();
    }

    private void LoadSpells()
    {
        //iterates through the spell slot images and assigns prepared spells from the inventory to these images
        for (int i = 0; i < spellSlotImages.Length; i++)
        {
            if (playerInventory.preparedSpells[i]!=null)
            spellSlotImages[i].sprite = playerInventory.preparedSpells[i].itemIcon;
        }
    }
}
