using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

//Helps manage and link together pieces of an enemy agent
public class EnemyAgentManager : MonoBehaviour
{
    //Enemy stats to manage
    public EnemyStats enemyStats;

    //Navigation agent attached
    public NavMeshAgent navAgent;

    //The targetted animator manager
    public AnimatorManager animatorManager;

    //The state machine connected to the manager
    public EnemyFSM stateMachine;
}
