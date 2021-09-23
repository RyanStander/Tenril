using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum RelationshipLevel
{
    Hostile = 1,
    Weary,
    Neutral,
    Receptive,
    Friendly
}

[CreateAssetMenu(fileName = "LeaderInformation", menuName = "Diplomacy/LeaderInformation", order = 1)]
public class LeaderInformation : ScriptableObject
{
    //Leader trait
    public string leaderName;
    public Sprite leaderIcon;
    public List<LeaderProductionTrait> leaderTraits;
    public RelationshipLevel currentRelationshipLevel = RelationshipLevel.Neutral;
}