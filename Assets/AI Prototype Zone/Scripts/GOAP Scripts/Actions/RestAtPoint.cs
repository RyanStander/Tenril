using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RestAtPoint : GAction
{
    public override bool PrePerform()
    {
        //If no target or at capacity, then fail to begin action
        if (target == null)
        {
            return false;
        }

        return true;
    }

    public override bool PostPerform()
    {
        //Declare that the agent is well rested
        agentBeliefs.ModifyState("WellRested", 1);

        return true;
    }
}