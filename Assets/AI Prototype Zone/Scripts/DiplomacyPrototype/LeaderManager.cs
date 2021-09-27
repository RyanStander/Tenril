using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;
using UnityEngine.UI;
using TMPro;

public class LeaderManager : MonoBehaviour
{
    //Leader Information
    public LeaderInformation leaderInfo;

    //UI Elements manager for the leader
    public LeaderVisualManager visualManager;

    //The state machine connected to the manager
    public DiplomacyFSM stateMachine;

    //The player being interacted with
    public PlayerProduction playerProduction;

    //Text that should be modified when making a statement to the player
    public TextMeshProUGUI statementText;

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
