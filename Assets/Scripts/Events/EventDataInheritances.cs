using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//Event that informs subscribers of a debug log
public class SendDebugLog : EventData
{
    public readonly string debuglog;

    public SendDebugLog(string givenLog) : base(EventType.ReceiveDebug)
    {
        debuglog = givenLog;
    }
}

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

//Event that informs subscribers of a quickslot is being added
public class AddQuickslotItem : EventData
{
    public readonly QuickslotItem quickslotItem;
    public AddQuickslotItem(QuickslotItem quickslotItem) : base(EventType.AddQuickslotItem)
    {
        this.quickslotItem = quickslotItem;
    }
}

//Event that informs subscribers of a quickslot is being removed
public class RemoveQuickslotItem : EventData
{
    public readonly QuickslotItem quickslotItem;
    public RemoveQuickslotItem(QuickslotItem quickslotItem) : base(EventType.RemoveQuickslotItem)
    {
        this.quickslotItem = quickslotItem;
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
