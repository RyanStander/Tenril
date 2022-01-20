using UnityEngine;

/// <summary>
/// Handles the dead logic for the AI
/// </summary>
public class DeadState : AbstractStateFSM
{
    //Prefab used in dropping the enemy inventory
    public GameObject itemPickupPrefab = null;

    //Height offset for dropped items
    public float dropHeightOffset = 0.25f;

    public override void OnEnable()
    {
        base.OnEnable();
        stateType = StateTypeFSM.DEAD;
    }

    public override bool EnterState()
    {
        //Run based method
        enteredState = base.EnterState();

        if (enteredState)
        {
            //Debug message
            DebugLogString("ENTERED DEAD STATE");

            //Send out event to award player XP
            EventManager.currentManager.AddEvent(new AwardPlayerXP(enemyManager.enemyStats.xpToAwardOnDeath));

            //Drop the inventory
            DropInventory();
        }

        return enteredState;
    }

    public override void UpdateState()
    {
        //Index for the action layer
        int layerIndex = animatorManager.animator.GetLayerIndex("Actions");

        if (enteredState)
        {
            DebugLogString("UPDATING DEAD STATE");
        }

        //Check if any of the death animations are playing, otherwise force the dead animation
        if(!animatorManager.animator.GetCurrentAnimatorStateInfo(layerIndex).IsName("Death") && !animatorManager.animator.GetCurrentAnimatorStateInfo(layerIndex).IsName("Dead") && !animatorManager.animator.GetCurrentAnimatorStateInfo(layerIndex).IsName("Riposted"))
        {
            if (animatorManager.animator.GetCurrentAnimatorStateInfo(layerIndex).IsName("Riposted"))
                return;

            //Play the death animation
            enemyManager.animatorManager.animator.Play("Dead");
        }
    }

    public override bool ExitState()
    {
        //Run based method
        base.ExitState();

        //Debug message
        DebugLogString("EXITED DEAD STATE");

        //Return true
        return true;
    }


    public void DropInventory()
    {
        //If item pickup prefab doesnt exist, throw an error and return
        if(itemPickupPrefab == null) { throw new MissingReferenceException("Missing itemPickupPrefab in " + gameObject + " death state!");}

        //Check that the item pickup script is attached
        if (itemPickupPrefab.TryGetComponent(out ItemPickup pickupSphere))
        {
            //Declare a temporary position to instantiate on
            Vector3 dropPosition = gameObject.transform.position;

            //Update the height with an offset
            dropPosition.y += dropHeightOffset;

            //Iterate over all items in the inventory
            foreach(ItemInventory item in enemyManager.inventory.inventory)
            {
                //Update the component with new information
                pickupSphere.item = item.item;
                pickupSphere.amountOfItem = item.itemStackCount;

                //Instantiate the prefab
                Instantiate(itemPickupPrefab, dropPosition, Quaternion.identity);
            }
        }
        //Throw an error
        else
        {
            throw new MissingComponentException("Missing ItemPickup on itemPickupPrefab in " + gameObject + " death state!");
        }
    }
}
