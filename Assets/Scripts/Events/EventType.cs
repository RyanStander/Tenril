using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//Defines the different event types to be used in event data in enumeration form
public enum EventType 
{
    ReceiveDebug,
    ClickedPlaceableGUI,

    //Save and loading
    SaveData,
    LoadData,
    LoadPlayerCharacterData,

    //Player Stats
    UpdatePlayerStats,
    UpdatePlayerHealth,
    UpdatePlayerStamina,
    UpdatePlayerMoonlight,
    UpdatePlayerSunlight,
    
    //Player State
    ChangePlayerState,

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
    InitiateDropStack,
    CompleteDropStack,
    DestroyInventoryOptionHolders,
    RequestEquippedWeapons,

    //Dialogue
    InitiateDialogue,
    CeaseDialogue,
    SendDialogueData,
    SendDialogueSentence,
    SendDialogueOptions,
    SendStartingStringTableForDialogue,
    ShowNextSentence,
    SendDialogueNpcInfo,

    //Camera
    SwapToLockOnCamera,
    SwapToExplorationCamera,
    SwapToAimCamera,
    SwapToMenuCamera,
    SwapToNPCCamera,
    SwapToLeftLockOnTarget,
    SwapToRightLockOnTarget,

    //Misc
    SendTimeStrength,
    AwardPlayerXP,
    PlayerLevelUp,
    PlayerGainSkill,
    PlayerObtainedItem,
    PlayerKeybindsUpdates,
    PlayerToggleSpellcastingMode,
    PlayerHasDroppedItem,
    PlayInteractSound,

    //Inputs
    PlayerChangedInputDevice,
}
