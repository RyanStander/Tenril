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
    //Leader information
    public string leaderName;
    public Sprite leaderIcon;
    public List<LeaderProductionTrait> leaderTraits;
    public RelationshipLevel currentRelationshipLevel = RelationshipLevel.Neutral;

    //Phrases to say when at a certain relationship status
    public List<string> insultingStatements = new List<string>();
    public List<string> complimentaryStatements = new List<string>();
    public List<string> neutralStatements = new List<string>();
}