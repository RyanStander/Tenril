using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyConsumable : Item
{
    [Tooltip("Detemines whether the item is single or infinite use")]
    public bool isConsumable;

    [Tooltip("The fx when successfully using item")]
    public GameObject itemUsedFX;
    [SerializeField] private Vector3 itemUsedFXOffset;
    public string useAnimation;

    [Tooltip("The object that is displayed when consuming the item")]
    public GameObject consumablePrefab;

    protected GameObject instantiatedItemFX;
    public virtual void AttemptToUseItem(EnemyAgentManager enemyManager)
    {
        Debug.Log("Attempting to use item!");

        //Play the animation of using the item
        enemyManager.animatorManager.PlayTargetAnimation(useAnimation, true);

        //Hide the current weapon and display the item
        enemyManager.consumableManager.DisplayItem(enemyManager.weaponSlotManager, consumablePrefab);
    }

    public virtual void SuccessfullyUsedItem(EnemyAgentManager enemyManager)
    {
        Debug.Log("Item used successfuly");

        //Create the successful item use effect
        if (itemUsedFX != null)
        {
            //Creates a position based on offsets with the original objects location in mind
            Vector3 position = enemyManager.animatorManager.transform.position +
                enemyManager.animatorManager.transform.forward * itemUsedFXOffset.x +
                enemyManager.animatorManager.transform.right * itemUsedFXOffset.z +
                enemyManager.animatorManager.transform.up * itemUsedFXOffset.y;

            //Create item effect
            instantiatedItemFX = Instantiate(itemUsedFX, position, enemyManager.animatorManager.transform.rotation);
        }
    }
}