using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RestAtPoint : GAction
{
    public override bool PrePerform()
    {
        //If no target or incomplete path, fail
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