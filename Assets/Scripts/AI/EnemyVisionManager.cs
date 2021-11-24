using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class EnemyVisionManager : MonoBehaviour
{
    //Relevant attached manager
    private EnemyAgentManager enemyManager;

    private void Awake()
    {
        //Getter for relevant reference
        enemyManager = GetComponentInChildren<EnemyAgentManager>();

        //Nullcheck for missing, throw exception as this does not guarantee it will break the code, but is likely to
        if (enemyManager == null) throw new MissingComponentException("Missing EnemyAgentManager on " + gameObject + "!");
    }

    internal Dictionary<GameObject, float> GetListOfVisibleEnemyTargets(Transform originPoint, float radius, LayerMask characterLayer, LayerMask detectionBlockLayer)
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
            GameObject visionPoint = GetVisionPoint(hitCollider.gameObject);

            //Temporary float for rough distance between objects
            float distance = Vector3.Distance(visionPoint.transform.position, originPoint.position);

            if (IsTargetUnobstructed(visionPoint.transform, originPoint, detectionBlockLayer))
            {
                //Check if character stats are attached, ignore otherwise
                if (hitCollider.gameObject.TryGetComponent(out CharacterStats stats))
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

        //Return the dictionary of targets
        return targetsByDistance;
    }

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
        GameObject visionPoint = ExtensionMethods.FindChildWithTag(searchedObject, "VisionTargetPoint");

        //If no vision point was found, default to the target
        if (visionPoint == null)
        {
            visionPoint = searchedObject;
        }

        //Return with results
        return visionPoint;
    }
}
