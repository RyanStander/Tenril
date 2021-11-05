﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//Defines the different event types to be used in event data in enumeration form
public enum EventType 
{
    ReceiveDebug,
    ClickedPlaceableGUI,
    UpdateQuickslotDisplay,
    UpdateStatusEffectsDisplay,
    UpdateInventoryDisplay,
    UpdateWeaponDisplay,
    EquipWeapon,
    AddQuickslotItem,
    RemoveQuickslotItem,
    UseItem,
    DropItem,
    DestroyInventoryOptionHolders,
    InitiateDialogue,
    CeaseDialogue,
    SendDialogueData,
    SendDialogueSentence,
    SendDialogueOptions,
    SendStartingStringTableForDialogue,
    SwapToLockOnCamera,
    SwapToExplorationCamera,
    SwapToMenuCamera,
    SwapToNPCCamera,
    SwapToLeftLockOnTarget,
    SwapToRightLockOnTarget
}
