using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DiplomacyCompliment : DiplomacyStatementState
{
    public override void OnEnable()
    {
        base.OnEnable();
        stateType = DiplomacyFSMStateType.COMPLIMENT;
    }

    public override bool EnterState()
    {
        //Run based method
        enteredState = base.EnterState();

        if (enteredState)
        {
            //Debug message
            DebugLogString("ENTERED COMPLIMENT STATE");
        }

        return enteredState;
    }

    public override void UpdateState()
    {
        if (enteredState)
        {
            //Run based method
            DebugLogString("UPDATING COMPLIMENT STATE");

            //Get random compliment from leader & apply text change
            leaderManager.statementText.text = leaderManager.leaderInfo.leaderName + ": " + GetRandomListString(leaderManager.leaderInfo.complimentaryStatements);

            //Return to idle state
            finiteStateMachine.EnterState(DiplomacyFSMStateType.IDLE);
        }
    }

    public override bool ExitState()
    {
        //Run based method
        base.ExitState();

        //Debug message
        DebugLogString("EXITED COMPLIMENT STATE");

        //Return true
        return true;
    }
}