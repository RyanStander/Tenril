using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DeathState : AbstractStateFSM
{
    public override void OnEnable()
    {
        base.OnEnable();
        stateType = StateTypeFSM.DEAD;
    }

    public override void UpdateState()
    {
        if (enteredState)
        {
            DebugLogString("UPDATING DEAD STATE");
        }
    }
}
