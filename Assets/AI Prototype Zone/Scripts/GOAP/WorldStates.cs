using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class WorldState
{
    //Pairs that assosiate a state and value
    public string key;
    public int value;
}

public class WorldStates
{
    //Links the states together
    public Dictionary<string, int> states;

    public WorldStates()
    {
        states = new Dictionary<string, int>();
    }

    //Check for if the state exists
    public bool HasState(string key)
    {
        return states.ContainsKey(key);
    }

    //Add a state and assosiated key to the dictionary
    void AddState(string key, int value)
    {
        states.Add(key, value);
    }

    //Modify an assosiated dictinary entrys value
    public void ModifyState(string key, int value)
    {
        if (states.ContainsKey(key))
        {
            states[key] += value;

            //Prevents negatives
            if (states[key] <= 0)
            {
                RemoveState(key);
            }
        }
        else
        {
            states.Add(key, value);
        }
    }

    //Remove the key pair from the dictionary
    public void RemoveState(string key)
    {
        if (states.ContainsKey(key))
        {
            states.Remove(key);
        }
    }

    //Set the value to a state
    public void SetState(string key, int value)
    {
        if (states.ContainsKey(key))
        {
            states[key] = value;
        }
        else
        {
            states.Add(key, value);
        }
    }

    //Return all available states
    public Dictionary<string, int> GetStates()
    {
        return states;
    }
}
