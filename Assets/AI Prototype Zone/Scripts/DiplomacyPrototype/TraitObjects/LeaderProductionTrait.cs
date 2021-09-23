using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "LeaderTrait", menuName = "Diplomacy/ProductionTrait", order = 2)]
public class LeaderProductionTrait : ScriptableObject
{
    //General
    public string traitName; //Name of the trait
    public string traitDescription; //Description of the trait

    //Production related
    public ProductionType targettedProduction; //The trait that is targetted
    public bool isFavoredProductionType; //Wether the target should favored or disliked by the leader
    public bool canLackProductionType; //Wether lacking in the target can negatively impact the player.
    [Range(1,2)] public float thoughtScoreMultiplier; //How much this trait affects the AI's likability of the player

    //Return in small digits
    public float CalculateLikability(float playerProduction, int maximumProduction)
    {
        //If the player could "lack" a production type, apply halfway markers
        if(canLackProductionType)
        {
            //Halve the maximum production and subtract from current production to allow for negatives
            maximumProduction /= 2;
            playerProduction -= maximumProduction;
        }

        //Temporary flaot to track likability, calcualte using production and modifiers
        float favorability = (playerProduction / maximumProduction) * thoughtScoreMultiplier;

        //Make the trait negatively impact the AI view on the player if trait is not favored
        if(!isFavoredProductionType)
        {
            favorability *= -1;
        }

        Debug.Log("Current favorability: " + favorability);

        return favorability;
    }
}
