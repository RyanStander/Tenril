using Interactables;
using UnityEngine;

/// <summary>
/// Handles the dead logic for the AI
/// </summary>
public class DeadState : AbstractStateFSM
{
    //Prefab used in dropping the enemy inventory
    public GameObject itemPickupPrefab = null;

    //Height offset for dropped items
    public float dropHeightOffset = 1f;

    //Helper bool for explosion item drop effect
    public bool explodeItemsOnDeath = true;

    //The velocity at which the item is launched into the air
    [Range(0,10)] public float upwardVelocity = 5;

    //The velocity at which the item is launched forward
    [Range(0, 2)] public float forwardVelocity = 0.75f;

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

            //Play the death animation
            enemyManager.animatorManager.animator.Play("Death");

            //Send out event to award player XP
            EventManager.currentManager.AddEvent(new AwardPlayerXp(enemyManager.enemyStats.xpToAwardOnDeath));

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
                GameObject droppedItem = Instantiate(itemPickupPrefab, dropPosition, Quaternion.identity);

                //Apply explosion effect to the rigid body of the dropped item
                if(droppedItem.TryGetComponent(out Rigidbody droppedRigidbody) && explodeItemsOnDeath)
                {
                    //Generate a random direction
                    Vector2 randomDirection = Random.insideUnitCircle.normalized;

                    //Apply given velocities velocity
                    droppedRigidbody.AddForce(new Vector3(randomDirection.x * forwardVelocity, upwardVelocity, randomDirection.y * forwardVelocity), ForceMode.VelocityChange);
                }
                
            }
        }
        //Throw an error
        else
        {
            throw new MissingComponentException("Missing ItemPickup on itemPickupPrefab in " + gameObject + " death state!");
        }
    }
}
