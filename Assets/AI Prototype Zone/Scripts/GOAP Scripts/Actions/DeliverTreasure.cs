using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class DeliverTreasure : GAction
{
    public override bool PrePerform()
    {
        //If no target, then fail to begin action
        if (target == null)
        {
            return false;
        }
        return true;
    }

    public override bool PostPerform()
    {
        //Add treasure to delivery point
        target.GetComponent<DeliveryPoint>().AddTreasure(gAgent.inventory.totalTreasure);

        //Clear inventory of treasure
        gAgent.inventory.ClearTreasure();

        //Remove belief that the treasure is at capacity
        agentBeliefs.RemoveState("ReachedTreasureCapacity");

        //Remove belief that the treasure hunter has treasure
        agentBeliefs.RemoveState("HasTreasure");

        return true;
    }
}