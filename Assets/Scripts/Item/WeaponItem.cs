using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "Items/Weapon")]
public class WeaponItem : Item
{
    [Tooltip("The model that is created on the weapon")]
    public GameObject primaryWeaponModelPrefab;
    [Tooltip("State wether there is a second weapon")]
    public bool hasSecondaryWeapon;
    [Tooltip("The secondary weapon, if there is one")]
    public GameObject secondaryWeaponModelPrefab;
    [Tooltip("Used for when no weapon is equipped")] 
    public bool isUnarmed=false;

    [Header("Damage")]
    public int baseDamage = 25;
    public float weakAttackDamageMultiplier = 1;
    public float strongAttackDamageMultiplier = 2;
    public int sneakDamageMultiplier = 4;

    [Header("Idle Animations")]
    public string idleAnimation;

    [Header("Attack Animations")]
    public AttackSet attackSet;
    public List<string> weakAttacks;
    public List<string> strongAttacks;

    [Header("Stamina Costs")]
    public int baseStaminaCost;
    public float weakAttackCostMultiplier=1;
    public float strongAttackCostMultiplier=2;

    [Header("WeaponType")]
    public WeaponType weaponType;

    public enum WeaponType
    {
        meleeWeapon,
        rangedWeapon,
        castingWeapon
    }
}

