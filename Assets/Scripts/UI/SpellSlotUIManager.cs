using System.Collections;
using System.Collections.Generic;
using System.Linq;
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
}
