using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class QuickslotItem : Item
{
    [Tooltip("The maximum amount of the item that can be carried")]
    public int amountPerStack;
    [Tooltip("Detemines whether the item is single or infinite use")]
    public bool isConsumable;

    [Tooltip("The fx when successfully using item")]
    public GameObject itemUsedFX;
    [SerializeField] private Vector3 itemUsedFXOffset;
    public string quickslotUseAnimation;

    [Header("Item Description")]
    [Tooltip("Description of what the item do")]
    [TextArea]
    public string quickslotItemDescription;

    protected GameObject instantiatedItemFX;
    public virtual void AttemptToUseItem(AnimatorManager animatorManager, CharacterStats characterStats)
    {
        Debug.Log("Attempting to use item!");

        //Play the animation of using the item
        animatorManager.PlayTargetAnimation(quickslotUseAnimation, true);
    }

    public virtual void SuccessfullyUsedItem(AnimatorManager animatorManager, CharacterStats characterStats)
    {
        Debug.Log("item used successfuly");

        //Create the successful item use effect
        if (itemUsedFX != null)
        {
            //Creates a position based on offsets with the original objects location in mind
            Vector3 position = animatorManager.transform.position +
                animatorManager.transform.forward * itemUsedFXOffset.x +
                animatorManager.transform.right * itemUsedFXOffset.z +
                animatorManager.transform.up * itemUsedFXOffset.y;

            //Create item effect
            instantiatedItemFX = Instantiate(itemUsedFX, position, animatorManager.transform.rotation);
        }
    }
}
