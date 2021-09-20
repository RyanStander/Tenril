using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class CollectTreasure : GAction
{
    [Range(1,10)]
    public int collectionRate = 10;

    public override bool PrePerform()
    {
        //If no target or at capacity, then fail to begin action
        if (target == null || HasReachedCapacity())
        {
            return false;
        }
        return true;
    }

    public override bool PostPerform()
    {
        //Add treasure to the backpack
        gAgent.inventory.AddTreasure(collectionRate);

        //Check if at capacity
        if (HasReachedCapacity())
        {
            //Declare that backpack is full
            agentBeliefs.ModifyState("ReachedTreasureCapacity", 1);
        }

        //Add belief that they have treasure
        agentBeliefs.ModifyState("HasTreasure", 1);

        return true;
    }

    public bool HasReachedCapacity()
    {
        //Temporary inventory reference
        GInventory inventory = gAgent.inventory;

        //Check if backpack returns as full
        return inventory.HasReachedCapacity(inventory.totalTreasure, inventory.treasureCapacity);
    }
}