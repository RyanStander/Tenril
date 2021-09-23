using System.Collections.Generic;
using UnityEngine;

public class SubGoal
{
    //Dictionary to store the goals
    public Dictionary<string, int> sGoals;

    //Bool to store if a given goal should be removed upon completion
    public bool isRemovedOnCompletion;

    // Constructor
    public SubGoal(string goalName, int valuePair, bool shouldRemoveOnCompletion)
    {
        sGoals = new Dictionary<string, int>();
        sGoals.Add(goalName, valuePair);
        isRemovedOnCompletion = shouldRemoveOnCompletion;
    }
}
