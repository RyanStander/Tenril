using System.Collections.Generic;
using UnityEngine;

public class GPlanner
{
    //Construct a plan of actions
    public Queue<GAction> PlanActions(List<GAction> actions, Dictionary<string, int> goal, WorldStates beliefStates)
    {
        //Create a list for the usable actions
        List<GAction> usableActions = new List<GAction>();

        //If an action is achievable, then this will be added to the usable actions
        //This clears the list of unusable actions that would otherwise clutter the list
        foreach (GAction a in actions) {

            if (a.IsAchievable())
            {
                usableActions.Add(a);
            }
        }

        //List to track the created branches
        List<Node> branches = new List<Node>();

        //Create the first branch of the node graph that will be created
        Node start = new Node(null, 0.0f, GWorld.Instance.GetWorld().GetStates(), beliefStates.GetStates(), null);

        //See if a graph is buildable (path for the actions)
        bool success = BuildGraph(start, branches, usableActions, goal);

        //If not possible, fail
        if (!success)
        {
            Debug.Log("NO PLAN");
            return null;
        }

        //Begin finding the cheapest nodes to follow
        Node cheapest = null;

        //Loop over each, looking for the cheapest leaf to follow
        foreach (Node branch in branches)
        {
            //If nothing was selected as the cheapest yet, then use the branch as the first cheapest option
            if (cheapest == null)
            {
                cheapest = branch;
            }
            //If cheaper than the previous branch, use
            else if (branch.cost < cheapest.cost) 
            {
                cheapest = branch;
            }
        }

        //Loop back around
        //List of actions that are the result
        List<GAction> result = new List<GAction>();
        Node node = cheapest;

        //While the node and actions are not null
        while (node != null)
        {
            if (node.action != null)
            {
                //Place action at start of the list
                result.Insert(0, node.action);
            }

            //Set parent of the node
            node = node.parent;
        }

        //Queue of the actions to take
        Queue<GAction> queue = new Queue<GAction>();

        //Queue up new actions
        foreach (GAction a in result)
        {
            queue.Enqueue(a);
        }

        //Debug the current plan that the NPC will take
        Debug.Log("The Plan is: ");
        foreach (GAction a in queue)
        {
            Debug.Log("Q: " + a.actionName);
        }

        return queue;
    }

    //Build the node graph of the actions
    private bool BuildGraph(Node parent, List<Node> branches, List<GAction> usableActions, Dictionary<string, int> goal) {

        bool foundActionPath = false; //Bool to track wether or not a valid action path has been found

        //Iterate over each action that can currently be executed
        foreach (GAction action in usableActions)
        {
            //If the action can be achieved
            if (action.IsAchievableGiven(parent.state))
            {
                //Declare dictionary of the current state based on the parent
                Dictionary<string, int> currentState = new Dictionary<string, int>(parent.state);

                //Iterate over the action effects that are contained by the current states
                foreach (KeyValuePair<string, int> eff in action.aftereffects)
                {
                    //If the current state does not already contain it, add it
                    if (!currentState.ContainsKey(eff.Key))
                    {
                        currentState.Add(eff.Key, eff.Value);
                    }
                }

                //Create a new node based on the previous results
                Node node = new Node(parent, parent.cost + action.cost, currentState, action);

                //If the goal is achieved, then add as a valid branch and declare that the path was found
                if (GoalAchieved(goal, currentState))
                {
                    branches.Add(node);
                    foundActionPath = true;
                }
                //If no path was found, move onto the next node
                else
                {
                    //Create a subset for the action to take out the current action in the node
                    //This will allow for future graphs to be built without creating ubreakable loops or unnecesary repetition
                    List<GAction> subset = ActionSubset(usableActions, action);

                    //Build a new graph with this new subset 
                    //Recursive call
                    bool found = BuildGraph(node, branches, subset, goal);

                    //Set the path as found if one is discovered
                    if (found)
                    {
                        foundActionPath = true;
                    }
                }
            }
        }
        //Return that the path was found
        return foundActionPath;
    }

    private List<GAction> ActionSubset(List<GAction> actions, GAction actionToRemove)
    {
        //Declare list of actions to return
        List<GAction> subset = new List<GAction>();

        //Iterate over the list of actions
        foreach (GAction a in actions)
        {
            //Remove the action to remove from the list
            if (!a.Equals(actionToRemove))
            {
                subset.Add(a);
            }
        }

        //Return the subset
        return subset;
    }

    //Check for if a goal is achieved
    private bool GoalAchieved(Dictionary<string, int> goal, Dictionary<string, int> state)
    {
        //Iterate over each pair of goals
        foreach (KeyValuePair<string, int> g in goal)
        {
            //If the state does not contain the goal, return false
            if (!state.ContainsKey(g.Key))
            {
                return false;
            }
        }
        //Otherwise return true
        return true;
    }
}
