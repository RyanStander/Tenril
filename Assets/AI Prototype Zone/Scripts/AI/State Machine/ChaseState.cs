using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ChaseState : State
{
    public AttackState attackState;
    public bool isInAttackRange;
    public CharacterAttributes characterAttributes;

    private void Start()
    {
        //Fetch the character attributes
        characterAttributes = GetComponentInParent<CharacterAttributes>();
    }

    public override State RunCurrentState()
    {
        //Return attack state if the target is in range
        if (isInAttackRange)
        {
            return attackState;
        }
        else
        {
            return this;
        }
    }
 
}
