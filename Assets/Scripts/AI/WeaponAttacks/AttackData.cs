using UnityEngine;

/// <summary>
/// This scriptable object is in charge of holding the data for specific attacks that AI can use
/// </summary>
[CreateAssetMenu(menuName = "Enemy AI/Attack Data")]
public class AttackData : ScriptableObject
{
    //The assosiated attack animation name
    public string attackAnimation;

    //How favorable the attack is, each unit of weight can be thought of as an extra time this attack might be chosen
    [Range(1,10)] public int attackWeight = 1;

    //The time it takes for the enemy to "recover" from the attack
    public float recoveryTime = 2;

    //The angle at which the attack is considered viable when looking at the target
    public float attackAngle = 35;

    //The minimum and maximum distance needed in order to attack
    public float minimumDistanceNeededToAttack = 0;
    public float maximumDistanceNeededToAttack = 3;

    //Ideal distance for the attack (used in repositioning)
    public float mostDesirableDistance = 1.5f;
}
