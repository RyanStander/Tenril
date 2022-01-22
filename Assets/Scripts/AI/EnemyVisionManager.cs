using System.Collections.Generic;
using UnityEngine;

public class EnemyVisionManager : MonoBehaviour
{
    //Point at which vision raycasting happens
    public Transform pointOfVision;

    //Relevant attached manager
    private EnemyAgentManager enemyManager;

    //Masks for detection and vision blocking
    public LayerMask detectionBlockLayer = 1 << 9;
    public LayerMask characterLayer = 1 << 10;

    private void Awake()
    {
        //Getter for relevant reference
        enemyManager = GetComponentInChildren<EnemyAgentManager>();

        //Nullcheck for missing, throw exception as this does not guarantee it will break the code, but is likely to
        if (enemyManager == null) throw new MissingComponentException("Missing EnemyAgentManager on " + gameObject + "!");
    }

    internal Dictionary<GameObject, float> GetListOfVisibleExpectedTargets(Transform originPoint, float radius, Faction targetFaction, bool isAlliedSearch)
    {
        //Cast a sphere wrapping the head and check for characters within range
        Collider[] hitColliders = Physics.OverlapSphere(originPoint.position, radius, characterLayer);

        //Temporary dictionary of targets by direct distance
        Dictionary<GameObject, float> targetsByDistance = new Dictionary<GameObject, float>();

        //Check each character for if it is within valid distance and for its faction
        foreach (var hitCollider in hitColliders)
        {
            //Continue (skip) execution if the found object was self, while avoiding any break to the foreach
            if (hitCollider.transform.gameObject == gameObject) continue;

            //If existing, get the vision point of the object, otherwise default to the hit collider
            Transform visionPoint = GetVisionPoint(hitCollider.gameObject).transform;

            //Temporary float for rough distance between objects
            float distance = Vector3.Distance(visionPoint.position, originPoint.position);

            if (IsTargetUnobstructed(visionPoint, originPoint, detectionBlockLayer))
            {
                //Check if character stats are attached, ignore otherwise
                if (hitCollider.gameObject.TryGetComponent(out CharacterStats stats))
                {
                    if(isAlliedSearch)
                    {
                        //Check for allied faction or no faction as well as for if the target is still alive
                        if (!stats.isDead)
                        {
                            if (targetFaction == stats.assignedFaction || targetFaction == Faction.NONE)
                            {
                                //If valid, add to dictionary
                                targetsByDistance.Add(hitCollider.gameObject, distance);
                            }
                        }
                    }
                    else
                    {
                        //Check for enemy faction or no faction as well as for if the target is still alive
                        //If valid, add to dictionary with unpathed & direct distance
                        if (!stats.isDead && stats.assignedFaction != enemyManager.enemyStats.assignedFaction && stats.assignedFaction != Faction.NONE)
                        {
                            enemyManager.stateMachine.DebugLogString("Target character is an enemy: " + hitCollider.gameObject + "| Distance: " + distance);
                            targetsByDistance.Add(hitCollider.gameObject, distance);
                        }
                    }
                }
            }
        }

        //Return the dictionary of targets
        return targetsByDistance;
    }

    //internal List<GameObject> GetListOfNearbyCharactersNoVision(Transform originPoint, float radius, Faction targetFaction)
    //{
    //    //Cast a sphere wrapping the head and check for characters within range
    //    Collider[] hitColliders = Physics.OverlapSphere(originPoint.position, radius, characterLayer);

    //    //Temporary list of targets
    //    List<GameObject> targetsToReturn = new List<GameObject>();

    //    //Check each character for if it is within valid distance and for its faction
    //    foreach (var hitCollider in hitColliders)
    //    {
    //        //Continue (skip) execution if the found object was self, while avoiding any break to the foreach
    //        if (hitCollider.transform.gameObject == gameObject) continue;

    //        //Check if character stats are attached, ignore otherwise
    //        if (hitCollider.gameObject.TryGetComponent(out CharacterStats stats))
    //        {
    //            //Check for allied faction or no faction as well as for if the target is still alive
    //            if (!stats.isDead)
    //            {
    //                if (targetFaction == stats.assignedFaction || targetFaction == Faction.NONE)
    //                {
    //                    //If valid, add to list
    //                    targetsToReturn.Add(hitCollider.gameObject);
    //                }
    //            }
    //        }
    //    }

    //    //Return the list of targets
    //    return targetsToReturn;
    //}

    //Checks for line of sight

    internal bool IsTargetUnobstructed(Transform targetPoint, Transform originPoint, LayerMask detectionBlockLayer)
    {
        enemyManager.stateMachine.DebugLogString("Checking for line of sight with character " + targetPoint.name);

        //Raycast to first check if the target is valid (Performance friendly to raycast first), checks if any objects are in the way
        if (!Physics.Raycast(originPoint.position, targetPoint.transform.position - originPoint.position, out RaycastHit hit, Vector3.Distance(targetPoint.transform.position, originPoint.position), detectionBlockLayer))
        {
            //Debug the hit result
            enemyManager.stateMachine.DebugLogString("No obstacles detected!");

            //Debug the line results
            Debug.DrawRay(originPoint.position, targetPoint.transform.position - originPoint.position, Color.green);

            //Return as unobstructed
            return true;
        }
        else
        {
            //Debug what was collided with
            enemyManager.stateMachine.DebugLogString("Failed to see target, obstacle detected: " + hit.transform.name);

            //Debug the line results
            Debug.DrawRay(originPoint.position, hit.point - originPoint.position, Color.red);

            //Return as obstructed
            return false;
        }
    }

    //Get the vision point (if applicable)
    private GameObject GetVisionPoint(GameObject searchedObject)
    {
        //If existing, get the vision point of the object, otherwise default to the searched object
        GameObject visionPoint = ExtensionMethods.DeepFindChildWithTag(searchedObject, "VisionTargetPoint");

        //If no vision point was found, default to the target
        if (visionPoint == null)
        {
            visionPoint = searchedObject;
        }

        //Return with results
        return visionPoint;
    }
}
