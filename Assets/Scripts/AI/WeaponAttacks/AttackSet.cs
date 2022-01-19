using System.Collections.Generic;
using UnityEngine;

//Meant to be used by the AI, allows for more advanced decision making and specializing movesets to different weapons
[CreateAssetMenu(menuName = "Enemy AI/Attack Set")]
public class AttackSet : ScriptableObject
{
    //List of light attack data
    public List<AttackData> lightAttacks = new List<AttackData>();

    //List of heavy attack data
    public List<AttackData> heavyAttacks = new List<AttackData>();

    //Iterates over the entire list of attacks and gets the maximum distance that the weapon functions at
    public float GetMaximumAttackRange()
    {
        //Temporary variable to track maximum range
        float maximumAttackRange = 0;

        //Iterate over all attacks and check for their attack ranges
        foreach(AttackData attack in lightAttacks)
        {
            //If greater than current attack range, update
            if(attack.maximumDistanceNeededToAttack > maximumAttackRange)
            {
                maximumAttackRange = attack.maximumDistanceNeededToAttack;
            }
        }
        foreach (AttackData attack in heavyAttacks)
        {
            //If greater than current attack range, update
            if (attack.maximumDistanceNeededToAttack > maximumAttackRange)
            {
                maximumAttackRange = attack.maximumDistanceNeededToAttack;
            }
        }

        //Return max range
        return maximumAttackRange;
    }
}
