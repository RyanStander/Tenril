using System.Collections;
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
}
