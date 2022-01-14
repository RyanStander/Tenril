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
            HealingLogic();
        }
    }

    private void HealingLogic()
    {
        //Return if already running an animation
        if (animatorManager.animator.GetBool("isInteracting")) return;

        //Check if creature should stop healing
        if(!enemyManager.ShouldTryHealing())
        {
            //Change states to evaluation to re-assess the situation
            finiteStateMachine.EnterState(StateTypeFSM.EVALUATECOMBAT);
        }
        //Check for available charges and return true if applicable
        else if (enemyManager.consumableManager.SelectHealingItem())
        {
            //Attempt to use the item
            enemyManager.inventory.consumableItemInUse.AttemptToUseItem(enemyManager.animatorManager, enemyManager.consumableManager, enemyManager.enemyStats);
        }
        else
        {
            //Mark healing as not possible
            enemyManager.enemyStats.canHeal = false;

            //Change states to evaluation to re-assess the situation
            finiteStateMachine.EnterState(StateTypeFSM.EVALUATECOMBAT);
        }
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
