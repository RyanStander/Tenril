using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System;

public class LeaderVisualManager : MonoBehaviour
{
    //UI objects for the leader
    public Image leaderIcon;
    public TextMeshProUGUI leaderName;
    public TextMeshProUGUI leaderRelationship;
    public TextMeshProUGUI leaderTraits;

    //Called when UI elements need updating
    public void UpdateVisuals(LeaderInformation leaderInfo)
    {
        //Use direct leader info to update visuals
        UpdateVisuals(
                leaderInfo.leaderIcon,
                leaderInfo.leaderName,
                leaderInfo.leaderTraits,
                Enum.GetName(typeof(RelationshipLevel),
                leaderInfo.currentRelationshipLevel));
    }
    public void UpdateVisuals(Sprite icon, string name, List<LeaderProductionTrait> traits, string relationship)
    {
        //Update name & icon
        leaderName.text = name;
        leaderIcon.sprite = icon;
        leaderRelationship.text = relationship;

        //Update traits list
        leaderTraits.text = "";
        foreach(LeaderProductionTrait trait in traits)
        {
            leaderTraits.text += "---------------" + "\n";
            leaderTraits.text += trait.traitName + "\n";
        }
    }
}
