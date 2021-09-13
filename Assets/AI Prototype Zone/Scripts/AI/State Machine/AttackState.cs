using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AttackState : State
{
    public ChaseState chaseState = null; //Chase state of the object
    public CharacterAttributes characterAttributes; //The characters attributes for relevant functions
    public GameObject targetObject = null; //The target of the object

    private void Start()
    {
        //Attempt to get the character attributes if null
        if (characterAttributes is null)
        {
            //Fetch the character attributes
            characterAttributes = GetComponentInParent<CharacterAttributes>();
        }

        //Give warning if target object is null
        if (targetObject is null)
        {
            //Debug error
            Debug.LogError("No target object attached!");
        }
    }

    public override State RunCurrentState()
    {
        //Return chase state if the target is out of attack range
        if (isInAttackRange())
        {
            //Otherwise fully run the state before returning
            RunState();
            return this;
        }
        else
        {
            return chaseState;
        }
    }

    public override void RunState()
    {
        //Temporary change to the material as a way to visualize changes in state
        transform.parent.GetComponent<Renderer>().material.color = Color.red;
    }

    public override void StopState()
    {
        throw new System.NotImplementedException();
    }

    private bool isInAttackRange()
    {
        //If the target is within attack range
        if (Vector3.Distance(transform.position, targetObject.transform.position) < characterAttributes.attackRange)
        {
            return true;
        }
        else
        {
            return false;
        }
    }
}
