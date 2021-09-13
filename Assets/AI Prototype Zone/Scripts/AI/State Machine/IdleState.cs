using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class IdleState : State
{
    public ChaseState chaseState;
    public CharacterAttributes characterAttributes; //The characters attributes for relevant functions
    public GameObject targetObject = null; //The target of the object

    public override State RunCurrentState()
    {
        //Return the chase state if target is within range
        if(isInChaseRange())
        {
            return chaseState;
        }
        else
        {
            //Otherwise fully run the state before returning
            RunState();
            return this;
        }
    }

    public override void RunState()
    {
        //Temporary change to the material as a way to visualize changes in state
        transform.parent.GetComponent<Renderer>().material.color = Color.gray;
    }

    public override void StopState() 
    {
        throw new System.NotImplementedException();
    }

    private bool isInChaseRange()
    {
        //If the target is within attack range
        if (Vector3.Distance(transform.position, targetObject.transform.position) < characterAttributes.chaseRange)
        {
            return true;
        }
        else
        {
            return false;
        }
    }
}
