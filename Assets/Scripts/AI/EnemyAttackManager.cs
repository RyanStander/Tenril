using UnityEngine;

/// <summary>
/// Purpose of this class is to track basic information related to an enemies current attack logic
/// Includes tracking desired distance, the current attack and an attack timeout for when they should reconsider an attack
/// </summary>
public class EnemyAttackManager : MonoBehaviour
{
    //Current time to wait before reassesing the available attacks
    [Range(1, 5)] public float timeoutTime = 5f;
    private float timeoutTimer = 0;
    public bool hasTimedOut = true;

    //Current attack trying to be performed
    public AttackData desiredAttack;

    //The current desired distance
    public float desiredDistance = 0;

    //The range of max and minimum ranges for the attack, used for when ideal distance is not reached, but the attack can still be made
    public Vector2 distanceBoundaries = new Vector2();

    //The offset amount at which the desired distance is considered to have been reached
    [Range(0.05f, 0.5f)] public float repositioningOffset = 0.1f;

    //Called by states to reset the attack management process
    public void TrackAttackData(AttackData givenAttack)
    {
        //Save the current attack being tracked
        desiredAttack = givenAttack;

        //Calculate the current desired distance based on the average 
        desiredDistance = givenAttack.mostDesirableDistance;

        //Update the distance boundaries
        distanceBoundaries = new Vector2(givenAttack.minimumDistanceNeededToAttack, givenAttack.maximumDistanceNeededToAttack);

        //Reset the timeout timer and boolean
        timeoutTimer = timeoutTime;
        hasTimedOut = false;
    }


    // Update is called once per frame
    private void FixedUpdate()
    {
        AttackTimeoutClock();
    }

    private void AttackTimeoutClock()
    {
        //Countdown the attack timeout
        if(timeoutTimer > 0)
        {
            //Decrease by time
            timeoutTimer -= Time.deltaTime;

            //Mark as still running
            hasTimedOut = false;
        }
        else
        {
            //Mark as timed out
            hasTimedOut = true;
        }
    }

    public bool IsWithinDesiredOffset(float currentDistance)
    { 
        //Check if within upper and lower bounds of the offset
        if (currentDistance > desiredDistance - repositioningOffset && currentDistance < desiredDistance + repositioningOffset)
        {
            return true;
        }
        else
        {
            return false;
        }
    }

    public bool IsWithinBoundaries(float currentDistance)
    {
        //Check if within upper and lower bounds of the available range
        if (currentDistance >= distanceBoundaries.x && currentDistance <= distanceBoundaries.y)
        {
            return true;
        }
        else
        {
            return false;
        }
    }
}
