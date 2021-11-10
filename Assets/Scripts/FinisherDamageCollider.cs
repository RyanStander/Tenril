using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FinisherDamageCollider : MonoBehaviour
{
    public Transform criticalDamageStandPosition;

    private float timeElapsed, lerpDuration = 0.5f;
    private bool currentlyMovingToPosition;
    private Vector3 startPoint;

    public void LerpToPoint(Transform transformToMove)
    {
        startPoint = transformToMove.position;
        currentlyMovingToPosition = true;
        timeElapsed = 0;
        while (currentlyMovingToPosition)
        {
            if (timeElapsed < lerpDuration)
            {
                transformToMove.position = Vector3.Lerp(startPoint, criticalDamageStandPosition.position, timeElapsed / lerpDuration);

                timeElapsed += Time.deltaTime;
            }
            else
            {
                currentlyMovingToPosition = false;
            }
        }
    }
}
