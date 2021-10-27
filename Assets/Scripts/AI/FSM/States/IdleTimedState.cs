using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class IdleTimedState : AbstractStateFSM
{
    [SerializeField]
    private float idleDuration = 5;
    private float totalDuration;

    public override void OnEnable()
    {
        base.OnEnable();
        stateType = StateTypeFSM.IDLE;
    }

    public override bool EnterState()
    {
        //Run based method
        enteredState = base.EnterState();

        if(enteredState)
        {
            //Debug message
            DebugLogString("ENTERED IDLE STATE");

            totalDuration = 0;
        }

        return enteredState;
    }

    public override void UpdateState()
    {
        if(enteredState)
        {
            DebugLogString("ENTERED IDLE STATE");
            totalDuration += Time.deltaTime;

            //If duration is passed, execute state change
            if(totalDuration >= idleDuration)
            {
                //Currently no state change implemented
                //finiteStateMachine.EnterState();
            }
        }
    }

    public override bool ExitState()
    {
        //Run based method
        base.ExitState();

        //Debug message
        DebugLogString("EXITED IDLE STATE");

        //Return true
        return true;
    }
}