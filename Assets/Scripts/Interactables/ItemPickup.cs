using Player;

namespace Interactables
{
    public class ItemPickup : Interactable
    {
        public Item item;
        public int amountOfItem=1;

        public override void Interact(PlayerManager playerManager)
        {
            base.Interact(playerManager);

            PickUpItem(playerManager);
        }

        protected virtual void PickUpItem(PlayerManager playerManager)
        {
            //Plays the animation of picking up item
            playerManager.playerAnimatorManager.PlayTargetAnimation("PickUp", true);

            //Add Item to inventory
            playerManager.playerInventory.AddItemToInventory(item,amountOfItem);

            EventManager.currentManager.AddEvent(new PlayerObtainedItem(item,amountOfItem));

            //Destroy the interactable after pick up
            Destroy(gameObject);
        }
    }
}
