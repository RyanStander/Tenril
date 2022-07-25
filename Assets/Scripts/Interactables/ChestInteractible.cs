using Interactables;
using Player;
using UnityEngine;

public class ChestInteractible : Interactable
{
    //Prefab used in dropping the enemy inventory
    public GameObject itemPickupPrefab = null;

    //Height offset for dropped items
    public float dropHeightOffset = 1f;

    //Helper bool for explosion item drop effect
    public bool explodeItemsOnOpening = true;

    //The velocity at which the item is launched into the air
    [Range(0, 10)] public float upwardVelocity = 5;

    //The velocity at which the item is launched forward
    [Range(0, 2)] public float forwardVelocity = 0.75f;

    //The current inventory that is attached to the chest
    private GenericInventory inventory;

    //Chest animator
    private Animator chestAnimator = null;

    //Layer that the chest should change to after being interacted with
    public LayerMask disabledLayer = 1 << 19;

    // Start is called before the first frame update
    void Start()
    {
        //Getters for relevant references
        inventory = GetComponentInChildren<GenericInventory>();
        chestAnimator = GetComponentInChildren<Animator>();

        //Nullcheck for missing
        if (inventory == null) throw new MissingComponentException("Missing Inventory on " + gameObject + "!");
        if (chestAnimator == null) throw new MissingComponentException("Missing Animator on " + gameObject + "!");

        //Load the inventory loot table
        inventory.PopulateInventoryItemTable();
    }

    public override void Interact(PlayerManager playerManager)
    {
        base.Interact(playerManager);

        //Plays the chest animation
        chestAnimator.SetBool("Activate", true);

        //Set the layer to prevent future interaction
        gameObject.layer = (int) Mathf.Log(disabledLayer.value, 2);
    }

    public void DropInventoryEvent()
    {
        //Drops the current inventory of the chest
        DropInventory();

        //Clears the chest inventory
        inventory.inventory.Clear();

        //Disabled this script to prevent future interaction
        this.enabled = false;
    }

    public void DropInventory()
    {
        //If item pickup prefab doesnt exist, throw an error and return
        if (itemPickupPrefab == null) { throw new MissingReferenceException("Missing itemPickupPrefab in " + gameObject + "!"); }

        //Check that the item pickup script is attached
        if (itemPickupPrefab.TryGetComponent(out ItemPickup pickupSphere))
        {
            //Declare a temporary position to instantiate on
            Vector3 dropPosition = gameObject.transform.position;

            //Update the height with an offset
            dropPosition.y += dropHeightOffset;

            //Iterate over all items in the inventory
            foreach (ItemInventory item in inventory.inventory)
            {
                //Update the component with new information
                pickupSphere.item = item.item;
                pickupSphere.amountOfItem = item.itemStackCount;

                //Instantiate the prefab
                GameObject droppedItem = Instantiate(itemPickupPrefab, dropPosition, Quaternion.identity);

                //Apply explosion effect to the rigid body of the dropped item
                if (droppedItem.TryGetComponent(out Rigidbody droppedRigidbody) && explodeItemsOnOpening)
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
            throw new MissingComponentException("Missing ItemPickup on itemPickupPrefab in " + gameObject + "!");
        }
    }
}
