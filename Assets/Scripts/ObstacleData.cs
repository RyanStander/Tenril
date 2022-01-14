using UnityEngine;

/// <summary>
/// This class is meant hold information relating to obstacles and their non mechanical properties
/// In its curren state it only serves to define the obstacle type, but would be extendable into including features such as custom impact sounds
/// </summary>
public class ObstacleData : MonoBehaviour
{
    //The type of obstacle that the object is
    public ObstacleType obstacleType;
}
