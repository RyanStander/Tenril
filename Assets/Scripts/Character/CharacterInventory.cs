using UnityEngine;

public class CharacterInventory : MonoBehaviour
{
    [Header("Equipped Items")]
    [Tooltip("The weapon the player currently has equipped")]
    public WeaponItem equippedWeapon;

    [Tooltip("Determines the weapon that the player currently has equiped and used for swapping")]
    public bool isWieldingPrimaryWeapon = true;

    [Tooltip("The 2 weapons that a player can swap between")]
    public WeaponItem primaryWeapon, secondaryWeapon;

    [Tooltip("The Ammo that the player will try to draw when using a ranged weapon")]
    public AmmunitionItem equippedAmmo;
}