using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DiplmacyInsult : DiplomacyStatementState
{
    public override void OnEnable()
    {
        base.OnEnable();
        stateType = DiplomacyFSMStateType.INSULT;
    }

    public override bool EnterState()
    {
        //Run based method
        enteredState = base.EnterState();

        if (enteredState)
        {
            //Debug message
            DebugLogString("ENTERED INSULT STATE");
        }

        return enteredState;
    }

    public override void UpdateState()
    {
        if (enteredState)
        {
            //Run based method
            DebugLogString("UPDATING INSULT STATE");

            //Get random compliment from leader & apply text change
            leaderManager.statementText.text = leaderManager.leaderInfo.leaderName + ": " + GetRandomListString(leaderManager.leaderInfo.insultingStatements);

            //Return to idle state
            finiteStateMachine.EnterState(DiplomacyFSMStateType.IDLE);
        }
    }

    public override bool ExitState()
    {
        //Run based method
        base.ExitState();

        //Debug message
        DebugLogString("EXITED INSULT STATE");

        //Return true
        return true;
    }
}