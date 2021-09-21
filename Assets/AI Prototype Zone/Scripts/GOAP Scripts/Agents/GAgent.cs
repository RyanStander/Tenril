using System.Collections.Generic;
using UnityEngine;
using System.Linq;
using UnityEngine.AI;

[RequireComponent(typeof(NavMeshAgent))]
[RequireComponent(typeof(GInventory))]
public class GAgent : MonoBehaviour
{
    public List<GAction> actions = new List<GAction>(); //List of actions that the agent has access to
    public Dictionary<SubGoal, int> goals = new Dictionary<SubGoal, int>(); //List of subgoals the agent will seek to achieve
    public WorldStates beliefs = new WorldStates(); //Agents internal states
    [HideInInspector] public GInventory inventory;
    private NavMeshAgent navAgent; //Although unused in this class, this should be used for action classes to fetch

    private GPlanner planner; //The planner
    private Queue<GAction> actionQueue; //Queue for the actions that will be taken
    public GAction currentAction; //The action that is currently being taken
    public SubGoal currentGoal; //Current goal that the agent will seek to achieve

    [Range(1.5f,2.5f)]
    public float goalDistanceSentitivity = 2f; //Distance for a goal to be valid

    // Start is called before the first frame update
    public void Start()
    {
        //Get the navmesh agent from the attached agent
        navAgent = gameObject.GetComponent<NavMeshAgent>();

        //Disable forced rotation
        navAgent.updateRotation = false;
        navAgent.updateUpAxis = false;

        //Create the array of actions that the agent has
        GAction[] availableActions = this.GetComponents<GAction>();
        foreach (GAction action in availableActions)
        {
            actions.Add(action);
        }

        //Get the inventory if one is attached
        if(gameObject.TryGetComponent(out GInventory inv))
        {
            inventory = inv;
        }
    }
    
    private bool invoked = false;
    public void CompleteAction()
    {
        //Finish running the action
        currentAction.running = false;

        //Method that is run at the end of an action
        currentAction.PostPerform();

        //Set invoked back to false, allowing for the next invoke test
        invoked = false;
    }

    // Update is called once per frame
    public void LateUpdate()
    {
        RunAgentLogic();
        CorrectSpriteOrientation();

        //If the path is partial, it means the path is no longer valid and the AI should re-evaluate their decisions
        //Debug.Log(navAgent.path.status);
        //Debug.Log(navAgent.pathStatus);
    }

    private void RunAgentLogic()
    {
        //If the current action is not currently executing
        if (currentAction != null && currentAction.running)
        {
            // Check the agent has a goal, a path to the goal that is valid, and has reached that goal
            if (currentAction.navAgent.hasPath && Vector3.Distance(currentAction.target.transform.position, transform.position) <= goalDistanceSentitivity)//currentAction.agent.remainingDistance <= goalDistanceSentitivity)
            {
                //If not yet invoked, then attempt to perform it
                if (!invoked)
                {
                    //Will complete the action after the actions duration
                    Invoke("CompleteAction", currentAction.duration);
                    invoked = true;
                }
            }
            return;
        }


        //If no plan or action queue exists, create one
        if (planner == null || actionQueue == null)
        {
            planner = new GPlanner();

            //Sort the goals by descending value
            var sortedGoals = from entry in goals orderby entry.Value descending select entry;

            //Loop through each goal and 
            foreach (KeyValuePair<SubGoal, int> sg in sortedGoals)
            {
                //Set the action queue to what the planner can return based on the goal
                actionQueue = planner.PlanActions(actions, sg.Key.sGoals, beliefs);

                //If the queue is not null then a plan exists
                if (actionQueue != null)
                {
                    // Set the current goal
                    currentGoal = sg.Key;

                    //Once the plan is found, break the loop
                    break;
                }
            }
        }

        //If a queue exists and the action queue is empty
        if (actionQueue != null && actionQueue.Count == 0)
        {
            //Check if the current goal can/should be removed
            if (currentGoal.isRemovedOnCompletion)
            {
                //Remove the goal
                goals.Remove(currentGoal);
            }
            //Nullify the planner in order to force the formation of a new plan
            planner = null;
        }

        //If there are still actions to execute in the queue
        if (actionQueue != null && actionQueue.Count > 0)
        {
            //Dequeue the top action of the queue and set it as the currentAction to execute
            currentAction = actionQueue.Dequeue();

            //Check for pre-conditions
            if (currentAction.PrePerform())
            {
                //If a target is not yet selected, find it
                if (currentAction.target == null && currentAction.targetTag != "")
                {
                    currentAction.target = GameObject.FindWithTag(currentAction.targetTag);
                }

                //If the target is not null (valid target)
                if (currentAction.target != null)
                {
                    //Start running the current action
                    currentAction.running = true;

                    //Set the destination for the navigation agent to move to
                    currentAction.navAgent.SetDestination(currentAction.target.transform.position);
                }

            }
            else
            {
                //Force a new plan by nullifying the queue
                actionQueue = null;
            }
        }
    }

    private void CorrectSpriteOrientation()
    {
        //Change based on agent velocity
        if (navAgent.velocity.x < 0)
        {
            GetComponent<SpriteRenderer>().flipX = false;
        }
        else if (navAgent.velocity.x > 0)
        {
            GetComponent<SpriteRenderer>().flipX = true;
        }
    }
}


