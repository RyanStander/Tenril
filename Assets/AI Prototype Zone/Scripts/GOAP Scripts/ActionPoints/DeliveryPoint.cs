using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DeliveryPoint : MonoBehaviour
{
    public int treasureCount; //The stored treasure at the delivery point

    public void AddTreasure(int givenTreasure)
    {
        treasureCount += givenTreasure;
    }

    //Returns any defecit that exists and clamps the point
    public int RemoveTreasure(int removedTreasure)
    {
        //Remove the amount
        treasureCount -= removedTreasure;

        //Defecit to calculate
        int defecit = 0;

        //Calculate if a defecit exists
        if (treasureCount < 0)
        {
            defecit = treasureCount;

            //Clamp the treasure count
            treasureCount = 0;
        }

        //Return defecit
        return defecit;
    }
}
