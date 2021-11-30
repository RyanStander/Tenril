using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//Defines the different event types to be used in event data in enumeration form
public enum EventType 
{
    ReceiveDebug,
    ClickedPlaceableGUI,

    //Player Stats
    UpdatePlayerStats,
    UpdatePlayerHealth,
    UpdatePlayerStamina,
    UpdatePlayerMoonlight,
    UpdatePlayerSunlight,

    //Inventory
    UpdateQuickslotDisplay,
    UpdateStatusEffectsDisplay,
    UpdateInventoryDisplay,
    UpdateWeaponDisplay,
    EquipWeapon,
    HideWeapon,
    ShowWeapon,
    AddQuickslotItem,
    RemoveQuickslotItem,
    DisplayQuickslotItem,
    HideQuickslotItem,
    RemoveItemFromInventory,
    UseItem,
    DropItem,
    DestroyInventoryOptionHolders,
    RequestEquippedWeapons,

    //Dialogue
    InitiateDialogue,
    CeaseDialogue,
    SendDialogueData,
    SendDialogueSentence,
    SendDialogueOptions,
    SendStartingStringTableForDialogue,

    //Camera
    SwapToLockOnCamera,
    SwapToExplorationCamera,
    SwapToMenuCamera,
    SwapToNPCCamera,
    SwapToLeftLockOnTarget,
    SwapToRightLockOnTarget,

    //Misc
    SendTimeStrength
}
