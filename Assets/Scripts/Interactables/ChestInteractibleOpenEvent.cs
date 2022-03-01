using UnityEngine;

/// <summary>
/// Intermediary class that helps toggle when the chest should drop its loot
/// </summary>
public class ChestInteractibleOpenEvent : MonoBehaviour
{
    //The interactible that will be called upon
    public ChestInteractible chestInteractible = null;

    //Call the inventory drop in chest interactible
    public void DropInventory()
    {
        //Silently fails as the chest might be a prop without functionality
        if (chestInteractible != null)
        {
            chestInteractible.DropInventoryEvent();
        }
    }
}
