public class DeadState : AbstractStateFSM
{
    public override void OnEnable()
    {
        base.OnEnable();
        stateType = StateTypeFSM.DEAD;
    }

    public override bool EnterState()
    {
        //Run based method
        enteredState = base.EnterState();

        if (enteredState)
        {
            //Debug message
            DebugLogString("ENTERED DEAD STATE");
        }

        return enteredState;
    }


    public override void UpdateState()
    {
        //Index for the action layer
        int layerIndex = animatorManager.animator.GetLayerIndex("Actions");

        if (enteredState)
        {
            DebugLogString("UPDATING DEAD STATE");
        }

        //Check if any of the death animations are playing, otherwise force the dead animation
        if(!animatorManager.animator.GetCurrentAnimatorStateInfo(layerIndex).IsName("Death") && !animatorManager.animator.GetCurrentAnimatorStateInfo(layerIndex).IsName("Dead"))
        {
            //Play the death animation
            enemyManager.animatorManager.animator.Play("Dead");
        }
    }

    public override bool ExitState()
    {
        //Run based method
        base.ExitState();

        //Debug message
        DebugLogString("EXITED DEAD STATE");

        //Return true
        return true;
    }
}
