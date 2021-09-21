using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public abstract class GAction : MonoBehaviour
{
    public string actionName = "Action"; //Name of the action
    public float cost = 1.0f; //Cost & favorability of the particular action
    public GameObject target; //Current target object for interaction
    public string targetTag; //Tag for the target
    public float duration = 0; //Duration of the action
    public WorldState[] antiConditions; //Set of conditions that prevents action from being possible
    public WorldState[] preConditions; //Action conditions that must be met before an action can take place
    public WorldState[] afterEffects; //The after effects of completing an action
    [HideInInspector] public NavMeshAgent navAgent; //The agent that will move
    [HideInInspector] public GAgent gAgent; //The GOAP attached agent

    public Dictionary<string, int> anticonditions; //Dictionary of anticonditions
    public Dictionary<string, int> preconditions; //Dictionary of preconditions
    public Dictionary<string, int> aftereffects; //Dictionary of effects

    [HideInInspector] public WorldStates agentBeliefs; //State of the agent itself (internal set of states)

    public bool running = false; //If the state is currently running

    public GAction()
    {
        //Create new dictionaries
        anticonditions = new Dictionary<string, int>();
        preconditions = new Dictionary<string, int>();
        aftereffects = new Dictionary<string, int>();
    }

    public void Awake()
    {
        //Get the navmesh agent
        navAgent = gameObject.GetComponent<NavMeshAgent>(); //gameObject.GetComponent<GAgent>().GetNavMeshAgent();
        gAgent = GetComponent<GAgent>();
        agentBeliefs = GetComponent<GAgent>().beliefs;

        //If anticonditions exists, add from world states
        if (antiConditions != null)
        {
            foreach (WorldState condition in antiConditions)
            {
                anticonditions.Add(condition.key, condition.value);
            }
        }

        //If preconditions exists, add from world states
        if (preConditions != null)
        {
            foreach (WorldState condition in preConditions)
            {
                preconditions.Add(condition.key, condition.value);
            }
        }

        //If after effect exists, add from world states
        if (afterEffects != null)
        {
            foreach (WorldState effect in afterEffects)
            {
                aftereffects.Add(effect.key, effect.value);
            }
        }
    }

    //If action can be completed
    public bool IsAchievable()
    {
        return true;
    }

    //If action can be completed based on set of pre-conditions or set of anti-conditions
    public bool IsAchievableGiven(Dictionary<string, int> conditions)
    {
        //Compare against preconditions
        foreach (KeyValuePair<string, int> pc in preconditions)
        {
            if (!conditions.ContainsKey(pc.Key))
            {
                return false;
            }
        }

        //Compare against anti-conditions
        foreach (KeyValuePair<string, int> ac in anticonditions)
        {
            if (conditions.ContainsKey(ac.Key))
            {
                return false;
            }
        }

        return true;
    }

    public abstract bool PrePerform();
    public abstract bool PostPerform();

    public GameObject GetClosestTarget(List<GameObject> targets)
    {
        //For now just return the first item
        return targets[0];
    }

    //Get if the path is possible
    public bool HasCompletePath()
    {
        if(navAgent.pathStatus == NavMeshPathStatus.PathComplete)
        {
            return true;
        } 
        else
        {
            return false;
        }
    }
}
