using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DiplomacyIdle : DiplomacyAbstractStateFSM
{
    //Helps track when the next turn button is pressed
    public bool isInteracted = false;

    public override void OnEnable()
    {
        base.OnEnable();
        stateType = DiplomacyFSMStateType.IDLE;
    }

    public override bool EnterState()
    {
        //Run based method
        enteredState = base.EnterState();

        if (enteredState)
        {
            //Debug message
            DebugLogString("ENTERED IDLE STATE");

            //Reset
            isInteracted = false;

            //Subscribe to turn button
            leaderManager.turnButton.onClick.AddListener(NextTurn);
        }

        return enteredState;
    }

    public override void UpdateState()
    {
        if (enteredState)
        {
            DebugLogString("UPDATING IDLE STATE");

            //Change state to evaluation mode if commanded to
            if (isInteracted)
            {
                finiteStateMachine.EnterState(DiplomacyFSMStateType.EVALUATING);
            }
        }
    }

    public override bool ExitState()
    {
        //Run based method
        base.ExitState();

        //Debug message
        DebugLogString("EXITED IDLE STATE");

        //Unsubscribe to turn button
        leaderManager.turnButton.onClick.RemoveListener(NextTurn);

        //Reset
        isInteracted = false;

        //Return true
        return true;
    }

    private void NextTurn()
    {
        DebugLogString("Called next turn!");
        if (enteredState) { isInteracted = true; }
    }
}