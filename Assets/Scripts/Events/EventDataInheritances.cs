using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Localization;

//Event that informs subscribers of a debug log
public class SendDebugLog : EventData
{
    public readonly string debuglog;

    public SendDebugLog(string givenLog) : base(EventType.ReceiveDebug)
    {
        debuglog = givenLog;
    }
}

#region Save And Loading

public class SaveData : EventData
{
    public SaveData() : base(EventType.SaveData)
    {

    }
}

public class LoadData : EventData
{
    public LoadData() : base(EventType.LoadData)
    {

    }
}

public class LoadPlayerCharacterData : EventData
{
    public readonly PlayerData playerData;
    public LoadPlayerCharacterData(PlayerData playerData) : base(EventType.LoadPlayerCharacterData)
    {
        this.playerData = playerData;
    }
}

#endregion

#region Player Stats Events

public class UpdatePlayerStats : EventData
{
    public readonly float playerMaxHealth, playerCurrentHealth;
    public readonly float playerMaxStamina, playerCurrentStamina;
    public readonly float playerMaxMoonlight, playerCurrentMoonlight;
    public readonly float playerMaxSunlight, playerCurrentSunlight;
    public UpdatePlayerStats (float playerMaxHealth, float playerCurrentHealth, float playerMaxStamina, float playerCurrentStamina, float playerMaxMoonlight, 
        float playerCurrentMoonlight , float playerMaxSunlight, float playerCurrentSunlight): base(EventType.UpdatePlayerStats)
    {
        this.playerMaxHealth = playerMaxHealth;
        this.playerCurrentHealth = playerCurrentHealth;

        this.playerMaxStamina = playerMaxStamina;
        this.playerCurrentStamina = playerCurrentStamina;

        this.playerMaxMoonlight = playerMaxMoonlight;
        this.playerCurrentMoonlight = playerCurrentMoonlight;

        this.playerMaxSunlight = playerMaxSunlight;
        this.playerCurrentSunlight = playerCurrentSunlight;
    }
}

public class UpdatePlayerHealth : EventData
{
    public readonly float playerMaxHealth, playerCurrentHealth;
    public UpdatePlayerHealth(float playerMaxHealth, float playerCurrentHealth) : base(EventType.UpdatePlayerHealth)
    {
        this.playerMaxHealth = playerMaxHealth;
        this.playerCurrentHealth = playerCurrentHealth;
    }
}

public class UpdatePlayerStamina : EventData
{
    public readonly float playerMaxStamina, playerCurrentStamina;
    public UpdatePlayerStamina(float playerMaxStamina, float playerCurrentStamina) : base(EventType.UpdatePlayerStamina)
    {
        this.playerMaxStamina = playerMaxStamina;
        this.playerCurrentStamina = playerCurrentStamina;
    }
}

public class UpdatePlayerMoonlight : EventData
{
    public readonly float playerMaxMoonlight, playerCurrentMoonlight;
    public UpdatePlayerMoonlight(float playerMaxMoonlight, float playerCurrentMoonlight) : base(EventType.UpdatePlayerMoonlight)
    {
        this.playerMaxMoonlight = playerMaxMoonlight;
        this.playerCurrentMoonlight = playerCurrentMoonlight;

    }
}

public class UpdatePlayerSunlight : EventData
{
    public readonly float playerMaxSunlight, playerCurrentSunlight;
    public UpdatePlayerSunlight( float playerMaxSunlight, float playerCurrentSunlight) : base(EventType.UpdatePlayerSunlight)
    {

        this.playerMaxSunlight = playerMaxSunlight;
        this.playerCurrentSunlight = playerCurrentSunlight;
    }
}
#endregion

#region Inventory Events
//Event that informs subscribers of the quickslot being updated
public class UpdateQuickslotDisplay : EventData
{
    public UpdateQuickslotDisplay() : base(EventType.UpdateQuickslotDisplay)
    {
    }
}

//Event that informs subscribers of the quickslot being updated
public class UpdateStatusEffectsDisplay : EventData
{
    public readonly StatusEffectManager statusEffectManager;
    public UpdateStatusEffectsDisplay(StatusEffectManager statusEffectManager) : base(EventType.UpdateStatusEffectsDisplay)
    {
        this.statusEffectManager = statusEffectManager;
    }
}

public class UpdateInventoryDisplay : EventData
{
    public UpdateInventoryDisplay() : base(EventType.UpdateInventoryDisplay)
    {
    }
}

//Event that informs subscribers of a weapon being equipped
public class EquipWeapon : EventData
{
    public readonly WeaponItem weaponItem;
    public readonly bool isPrimaryWeapon;
    public EquipWeapon(WeaponItem weaponItem, bool isPrimaryWeapon) : base(EventType.EquipWeapon)
    {
        this.weaponItem = weaponItem;
        this.isPrimaryWeapon = isPrimaryWeapon;
    }
}

public class HideWeapon : EventData
{
    public HideWeapon() : base(EventType.HideWeapon)
    {

    }
}

public class ShowWeapon : EventData
{
    public ShowWeapon() : base(EventType.ShowWeapon)
    {

    }
}

//Event that informs subscribers of a quickslot is being added
public class AddQuickslotItem : EventData
{
    public readonly ConsumableItem quickslotItem;
    public AddQuickslotItem(ConsumableItem quickslotItem) : base(EventType.AddQuickslotItem)
    {
        this.quickslotItem = quickslotItem;
    }
}

//Event that informs subscribers of a quickslot is being removed
public class RemoveQuickslotItem : EventData
{
    public readonly ConsumableItem quickslotItem;
    public RemoveQuickslotItem(ConsumableItem quickslotItem) : base(EventType.RemoveQuickslotItem)
    {
        this.quickslotItem = quickslotItem;
    }
}

public class DisplayQuickslotItem : EventData
{
    public readonly GameObject objectToDisplay;
    public DisplayQuickslotItem(GameObject objectToDisplay) : base(EventType.DisplayQuickslotItem)
    {
        this.objectToDisplay = objectToDisplay;
    }
}

public class HideQuickslotItem : EventData
{
    public HideQuickslotItem() : base(EventType.HideQuickslotItem)
    {

    }
}

public class RemoveItemFromInventory : EventData
{
    public readonly Item item;
    public readonly int amountToBeRemoved;
    public RemoveItemFromInventory(Item item,int amountToBeRemoved = 1) : base(EventType.RemoveItemFromInventory)
    {
        this.item = item;
        this.amountToBeRemoved = amountToBeRemoved;
    }
}

//Event that informs subscribers of an item being dropped
public class DropItem : EventData
{
    public readonly Item item;
    public DropItem(Item item) : base(EventType.DropItem)
    {
        this.item = item;
    }
}

public class InitiateDropStack : EventData
{
    public readonly Item item;
    public readonly int amountThatCanBeDropped;
    public InitiateDropStack(Item item, int amountThatCanBeDropped) : base(EventType.InitiateDropStack)
    {
        this.item = item;
        this.amountThatCanBeDropped = amountThatCanBeDropped;
    }
}

public class CompleteDropStack : EventData
{
    public readonly Item item;
    public readonly int amountToDrop;
    public CompleteDropStack(Item item, int amountToDrop) : base(EventType.CompleteDropStack)
    {
        this.item = item;
        this.amountToDrop = amountToDrop;
    }
}

public class PlayerHasDroppedItem : EventData
{
    public readonly Item item;
    public readonly int amountToDrop;
    public PlayerHasDroppedItem(Item item, int amountToDrop) : base(EventType.PlayerHasDroppedItem)
    {
        this.item = item;
        this.amountToDrop = amountToDrop;
    }
}

//Event that informs subscribers of an item being used
public class UseItem : EventData
{
    public readonly Item item;
    public UseItem(Item item) : base(EventType.UseItem)
    {
        this.item = item;
    }
}

//Event that informs subscribers of a inventory options being created
public class DestroyInventoryOptionHolders : EventData
{
    public DestroyInventoryOptionHolders() : base(EventType.DestroyInventoryOptionHolders)
    {
    }
}

//Event that informs subscribers of equipped weapons being modified
public class  UpdateWeaponDisplay : EventData
{
    public readonly WeaponItem primaryWeapon, secondaryWeapon;
    public readonly bool isWieldingPrimaryWeapon;
    public UpdateWeaponDisplay(WeaponItem primaryWeapon, WeaponItem secondaryWeapon, bool isWieldingPrimaryWeapon) : base(EventType.UpdateWeaponDisplay)
    {
        this.primaryWeapon = primaryWeapon;
        this.secondaryWeapon = secondaryWeapon;
        this.isWieldingPrimaryWeapon = isWieldingPrimaryWeapon;
    }
}

//Event that is used to ask inventory to send event on items
public class RequestEquippedWeapons : EventData
{
    public RequestEquippedWeapons() : base(EventType.RequestEquippedWeapons)
    {

    }
}

#endregion

#region Dialogue Events

//Event that informs subscribers of initiating dialogue with npc
public class InitiateDialogue : EventData
{
    public InitiateDialogue() : base(EventType.InitiateDialogue)
    {

    }
}

//Event that informs subscribers of ceasing dialoague with npc
public class CeaseDialogue : EventData
{
    public CeaseDialogue() : base(EventType.CeaseDialogue)
    {

    }
}

public class ShowNextSentence : EventData
{
    public ShowNextSentence() : base(EventType.ShowNextSentence)
    {

    }
}

//sends a list of dialogue for a specific interaction
public class SendDialogueData : EventData
{
    public readonly DialogueData dialogueData;
    public SendDialogueData(DialogueData dialogueData) : base(EventType.SendDialogueData)
    {
        this.dialogueData = dialogueData;
    }
}

public class SendStartingStringTableForDialogue : EventData
{
    public readonly LocalizedStringTable localizedStringTable;

        public SendStartingStringTableForDialogue(LocalizedStringTable localizedStringTable) : base(EventType.SendStartingStringTableForDialogue)
    {
        this.localizedStringTable = localizedStringTable;
    }
}

//sends a single string of dialogue to subscribers
public class SendDialogueSentence : EventData
{
    public readonly string npcName;
    public readonly string sentence;
    public SendDialogueSentence(string npcName,string sentence):base(EventType.SendDialogueSentence)
    {
        this.npcName = npcName;
        this.sentence = sentence;
    }
}

public class SendDialogueOptions : EventData
{
    public readonly List<string> options;
    public readonly List<DialogueData> nextDialogues;
    public SendDialogueOptions(List<string> options, List<DialogueData> nextDialogues) : base(EventType.SendDialogueOptions)
    {
        this.options = options;
        this.nextDialogues = nextDialogues;
    }
}

#endregion

#region Camera Events

public class SwapToLockOnCamera : EventData
{
    public SwapToLockOnCamera() : base(EventType.SwapToLockOnCamera)
    {

    }
}

public class SwapToExplorationCamera : EventData
{
    public SwapToExplorationCamera() : base(EventType.SwapToExplorationCamera)
    {

    }
}

public class SwapToAimCamera : EventData
{
    public SwapToAimCamera() : base(EventType.SwapToAimCamera)
    {
    }
}

public class SwapToMenuCamera : EventData
{
    public SwapToMenuCamera() : base(EventType.SwapToMenuCamera)
    {

    }
}

public class SwapToNPCCamera: EventData
{
    public readonly GameObject npcCamera;
    public SwapToNPCCamera(GameObject npcCamera) : base(EventType.SwapToNPCCamera)
    {
        this.npcCamera = npcCamera;
    }
}

public class SwapToLeftLockOnTarget : EventData
{
    public SwapToLeftLockOnTarget():base(EventType.SwapToLeftLockOnTarget)
    {
    }
}

public class SwapToRightLockOnTarget : EventData
{
    public SwapToRightLockOnTarget() : base(EventType.SwapToRightLockOnTarget)
    {
    }
}
#endregion

#region Misc

public class SendTimeStrength : EventData
{
    public readonly float timeStrength;
    public SendTimeStrength(float timeStrength) : base(EventType.SendTimeStrength)
    {
        this.timeStrength = timeStrength;
    }
}

public class AwardPlayerXP : EventData
{
    public readonly int xpAmount;
    public AwardPlayerXP(int xpAmount) : base(EventType.AwardPlayerXP)
    {
        this.xpAmount = xpAmount;
    }
}

public class PlayerLevelUp : EventData
{
    public readonly int amountOfLevelsGained;
    public PlayerLevelUp(int amountOfLevelsGained) : base(EventType.PlayerLevelUp)
    {
        this.amountOfLevelsGained = amountOfLevelsGained;
    }
}

public class PlayerGainSkill : EventData
{
    public readonly Skill skillToGain;
    public readonly bool consumeSkillPoint;
    public PlayerGainSkill(Skill skillToGain, bool consumeSkillPoint=true) : base(EventType.PlayerGainSkill)
    {
        this.skillToGain = skillToGain;
        this.consumeSkillPoint = consumeSkillPoint;
    }
}

/// <summary>
/// used for when the player has obained an item to display in the item log
/// </summary>
public class PlayerObtainedItem : EventData
{
    public readonly Item itemObtained;
    public readonly int amountObtained;

    public PlayerObtainedItem(Item itemObtained, int amountObtained=1) : base(EventType.PlayerObtainedItem)
    {
        this.itemObtained = itemObtained;
        this.amountObtained = amountObtained;
    }
}

public class PlayerKeybindsUpdate : EventData
{
    public PlayerKeybindsUpdate() : base(EventType.PlayerKeybindsUpdates)
    {
    }
}

public class PlayerToggleSpellcastingMode : EventData
{
    public bool enteredSpellcastingMode;
    public PlayerToggleSpellcastingMode(bool enteredSpellcastingMode) : base(EventType.PlayerToggleSpellcastingMode)
    {
        this.enteredSpellcastingMode = enteredSpellcastingMode;
    }
}

public class PlayInteractSound : EventData
{
    public PlayInteractSound() : base(EventType.PlayInteractSound)
    {
    }
}
#endregion

#region Inputs

public class PlayerChangedInputDevice : EventData
{
    public readonly InputDeviceType inputDevice;

    public PlayerChangedInputDevice(InputDeviceType inputDevice) : base(EventType.PlayerChangedInputDevice)
    {
        this.inputDevice = inputDevice;
    }
}

#endregion
