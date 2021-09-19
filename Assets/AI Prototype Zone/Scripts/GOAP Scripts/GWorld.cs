using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

public sealed class GWorld
{
    private static readonly GWorld instance = new GWorld(); //Singleton
    private static WorldStates world; //Dictionary of all our states

    public static List<GameObject> treasurePoints = new List<GameObject>(); //List of all existing treasure points
    public static List<GameObject> restingPoints = new List<GameObject>(); //List of all existing resting points
    public static List<GameObject> deliveryPoints = new List<GameObject>(); //List of all existing delivery points

    static GWorld()
    {
        //Create new dictionary of the states
        world = new WorldStates();

        //Get all the world treasure points and modify the state world to report the existing amount
        treasurePoints = GameObject.FindGameObjectsWithTag("TreasurePoint").ToList();
        if (treasurePoints.Count > 0)
        {
            world.ModifyState("TreasurePointExists", treasurePoints.Count);
        }

        //Get all the world delivery points and modify the state world to report the existing amount
        deliveryPoints = GameObject.FindGameObjectsWithTag("DeliveryPoint").ToList();
        if (deliveryPoints.Count > 0)
        {
            world.ModifyState("DeliveryPointExists", deliveryPoints.Count);
        }

        //Get all the world resting points and modify the state world to report the existing amount
        restingPoints = GameObject.FindGameObjectsWithTag("RestingPoint").ToList();
        if (restingPoints.Count > 0)
        {
            world.ModifyState("RestingPointExists", restingPoints.Count);
        }
    }

    private GWorld()
    {
    }

    //Return current instance
    public static GWorld Instance
    {
        get { return instance; }
    }

    //Return the states
    public WorldStates GetWorld()
    {
        return world;
    }
}
