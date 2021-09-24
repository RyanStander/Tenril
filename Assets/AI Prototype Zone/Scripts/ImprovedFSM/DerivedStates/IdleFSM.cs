using System.Collections;
using System.Collections.Generic;
using UnityEngine;


[CreateAssetMenu(fileName = "IdleFSM", menuName = "Unity-FSM/States/Idle", order = 1)]
public class IdleFSM : AbstractStateFSM
{
    [SerializeField]
    private float idleDuration = 5;
    private float totalDuration;

    public override void OnEnable()
    {
        base.OnEnable();
        stateType = FSMStateType.IDLE;
    }

    public override bool EnterState()
    {
        //Run based method
        enteredState = base.EnterState();

        if(enteredState)
        {
            //Debug message
            Debug.Log("ENTERED IDLE STATE");

            totalDuration = 0;
        }

        return enteredState;
    }

    public override void UpdateState()
    {
        if(enteredState)
        {
            Debug.Log("UPDATING IDLE STATE");
            totalDuration += Time.deltaTime;

            //Force patrol if idle should end
            if(totalDuration >= idleDuration)
            {
                finiteStateMachine.EnterState(FSMStateType.PATROL);
            }
        }
    }

    public override bool ExitState()
    {
        //Run based method
        base.ExitState();

        //Debug message
        Debug.Log("EXITED IDLE STATE");

        //Return true
        return true;
    }
}