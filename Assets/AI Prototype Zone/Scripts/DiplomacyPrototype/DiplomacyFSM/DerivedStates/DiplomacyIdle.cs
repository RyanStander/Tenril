using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "DiplomacyIdle", menuName = "Diplomacy/States/Idle", order = 1)]
public class DiplomacyIdle : DiplomacyAbstractStateFSM
{
    [SerializeField]
    private bool isInteracted = false;

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
                Debug.Log("Changing!");
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

        //Return true
        return true;
    }

    private void NextTurn()
    {
        Debug.Log("Called next turn!");
        if (enteredState) { isInteracted = true; }
    }
}