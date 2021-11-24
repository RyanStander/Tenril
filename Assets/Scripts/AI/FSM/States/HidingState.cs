using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HidingState : AbstractStateFSM
{
    //Masks for detection and vision blocking
    public LayerMask detectionBlockLayer = 1 << 9;

    //Bool for if a check rate should be used
    public bool isUsingCheckRate;

    //Performance related rate at which the agent should cast a check
    [Range(0, 1)]
    public float checkRate = 0.5f;

    public override void OnEnable()
    {
        base.OnEnable();
        stateType = StateTypeFSM.HIDE;
    }

    public override bool EnterState()
    {
        //Run based method
        enteredState = base.EnterState();

        if (enteredState)
        {
            //Debug message
            DebugLogString("ENTERED HIDE STATE");
        }

        return enteredState;
    }


    public override void UpdateState()
    {
        if (enteredState)
        {
            DebugLogString("UPDATING HIDE STATE");
        }
    }

    public override bool ExitState()
    {
        //Run based method
        base.ExitState();

        //Debug message
        DebugLogString("EXITED HIDE STATE");

        //Return true
        return true;
    }
}
