using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DestroyInventoryOptionHolderButtonFunction : MonoBehaviour
{
    public void DestroyInventoryOptionHolder()
    {
        EventManager.currentManager.AddEvent(new DestroyInventoryOptionHolders());    
    }
}
