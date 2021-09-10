using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ChaseState : State
{
    public AttackState attackState = null; //Attack state of the object
    public IdleState idleState = null; //Idle state of the object
    public CharacterAttributes characterAttributes; //The characters attributes for relevant functions
    public AbstractMovement currentMovement; //The current movement type attached
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
        //Return attack state if the target is in range
        if (isInAttackRange())
        {
            return attackState;
        }
        else if(isInChaseRange())
        {
            //Run the state before returning this
            RunState();
            return this;
        }
        else
        {
            //Otherwise return to idle state
            return idleState;
        }
    }

    public override void RunState()
    {
        //Move towards the intended target
        currentMovement.MoveTowardsTarget(characterAttributes, targetObject.transform.position);

        //Temporary change to the material as a way to visualize changes in state
        transform.parent.GetComponent<Renderer>().material.color = Color.yellow;
    }

    private bool isInAttackRange()
    {
        //If the target is within attack range
        if(Vector3.Distance(transform.position, targetObject.transform.position) < characterAttributes.attackRange)
        {
            return true;
        }
        else
        {
            return false;
        }
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
