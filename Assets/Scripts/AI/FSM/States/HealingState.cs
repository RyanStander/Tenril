using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HealingState : AbstractStateFSM
{
    public override void OnEnable()
    {
        base.OnEnable();
        stateType = StateTypeFSM.HEALING;
    }

    public override bool EnterState()
    {
        //Run based method
        enteredState = base.EnterState();

        if (enteredState)
        {
            //Debug message
            DebugLogString("ENTERED HEALING STATE");
        }

        return enteredState;
    }


    public override void UpdateState()
    {
        if (enteredState)
        {
            DebugLogString("UPDATING HEALING STATE");

            if(animatorManager.animator.GetBool("isInteracting"))
            {
                Debug.Log("Im busy healing");
            }
            else
            {
                Debug.Log("I'm not busy healing");
            }

            //For now and for testing, wait on a key input
            if (Input.GetKeyUp(KeyCode.Space))
            {
                HealingLogic();
            }
        }
    }

    private void HealingLogic()
    {
        //Return if already running an animation
        if (animatorManager.animator.GetBool("isInteracting")) return;

        //Check for available charges and return true if applicable
        if (enemyManager.consumableManager.SelectHealingItem())
        {
            //Attempt to use the item
            enemyManager.inventory.consumableItemInUse.AttemptToUseItem(enemyManager.animatorManager, enemyManager.weaponSlotManager, enemyManager.consumableManager, enemyManager.enemyStats);
        }
        else
        {
            //Mark healing as not possible
            enemyManager.enemyStats.canHeal = false;

            //Change states to evaluation to re-assess the situation
            finiteStateMachine.EnterState(StateTypeFSM.EVALUATECOMBAT);
        }

        //TODO: Lower item availability
        //Reduce the number of charges for the available healing item (if applicable)
    }

    public override bool ExitState()
    {
        //Run based method
        base.ExitState();

        //Debug message
        DebugLogString("EXITED HEALING STATE");

        //Return true
        return true;
    }
}
