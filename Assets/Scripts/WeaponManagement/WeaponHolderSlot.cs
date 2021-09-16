using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WeaponHolderSlot : MonoBehaviour
{
    /// <summary>
    /// Place these on areas where the weapons should be attached to, such as hands for blades, back for a back weapon, etc.
    /// Can make a child to override the placement of the weapon.
    /// </summary>
    [Header("Used for overriding where the weapon is loaded onto")]
    public Transform parentOverride;
    [Header("The weapon that is currently loaded")]
    public WeaponItem currentWeapon;

    [Header("The slot to assign the weapon to")]
    public WeaponSlot weaponSlot;

    public enum WeaponSlot
    {
        rightHand,
        leftHand,
        backSlot,
        leftSideSlot,
        rightSideSlot
    }
}
