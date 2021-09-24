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

        //Subscribe to turn button
        leaderManager.turnButton.onClick.AddListener(NextTurn);
    }

    public void OnDisable()
    {
        //Unsubscribe to turn button
        leaderManager.turnButton.onClick.RemoveListener(NextTurn);
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

        //Return true
        return true;
    }

    private void NextTurn()
    {
        if (enteredState) { isInteracted = true; }
    }
}