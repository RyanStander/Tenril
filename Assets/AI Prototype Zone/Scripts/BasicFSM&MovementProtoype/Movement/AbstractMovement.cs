using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class AbstractMovement : MonoBehaviour
{
    public abstract void MoveTowardsTarget(CharacterAttributes attributes, Vector3 targetPosition);
    public abstract void StopMovement();
}
