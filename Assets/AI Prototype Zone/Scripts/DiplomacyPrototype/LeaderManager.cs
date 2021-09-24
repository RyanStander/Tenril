using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class LeaderManager : MonoBehaviour
{
    //Leader Information
    public LeaderInformation leaderInfo;

    //UI Elements manager for the leader
    public LeaderVisualManager visualManager;


    private void Start()
    {
        UpdateVisuals();
    }

    public void UpdateVisuals()
    {
        //If visual manager exists, update
        if (visualManager != null)
        {
            visualManager.UpdateVisuals(
                leaderInfo.leaderIcon,
                leaderInfo.leaderName,
                leaderInfo.leaderTraits,
                Enum.GetName(typeof(RelationshipLevel),
                leaderInfo.currentRelationshipLevel));
        }
    }
}
