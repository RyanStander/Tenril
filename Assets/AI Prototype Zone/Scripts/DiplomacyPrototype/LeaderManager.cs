using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;
using UnityEngine.UI;

public class LeaderManager : MonoBehaviour
{
    //Leader Information
    public LeaderInformation leaderInfo;

    //UI Elements manager for the leader
    public LeaderVisualManager visualManager;

    //The state machine connected to the manager
    private DiplomacyFSM stateMachine;

    //The player being interacted with
    public PlayerProduction playerProduction;

    //Button used for triggering turn changes
    public Button turnButton;

    private void Start()
    {
        UpdateVisuals();
    }

    public void UpdateVisuals()
    {
        //If visual manager exists, update
        if (visualManager != null)
        {
            visualManager.UpdateVisuals(leaderInfo);
        }
    }
}
