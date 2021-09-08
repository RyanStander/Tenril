using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class IdleState : State
{
    public ChaseState chaseState;
    public bool canSeeTarget;

    public override State RunCurrentState()
    {
        //Return the chase state if target is visible
        if(canSeeTarget)
        {
            return chaseState;
        }
        else
        {
            return this;
        }
    }
}
