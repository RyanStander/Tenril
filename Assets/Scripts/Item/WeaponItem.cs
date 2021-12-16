using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "Items/Weapon")]
public class WeaponItem : Item
{
    [Header("Prefabs")]
    [Tooltip("The right model that is created on the weapon")]
    public GameObject rightWeaponModelPrefab;
    [Tooltip("The left model that is created on the weapon")]
    public GameObject leftWeaponModelPrefab;

    public bool displaySecondaryWeaponWhenUnequipped;
    [Tooltip("The model displayed when your weapon is not being weilded")]
    public GameObject unequippedPrimaryWeaponModelPrefab;
    [Tooltip("The secondary model displayed if you choose a side slot and it has a secondary weapon")]
    public GameObject unequippedSecondaryWeaponModelPrefab;
    [Tooltip("The sheath will be present at all times, usually placed in a way that the weapon fits inside it")]
    public GameObject primarySheathPrefab;
    public GameObject secondarySheathPrefab;

    [Header("Animator Replacer")]
    public AnimatorOverrideController weaponController;

    [Header("Sound Data")]
    public WeaponSoundEffects weaponSoundEffects;

    [Header("Damage")]
    public int baseDamage = 25;
    public float weakAttackDamageMultiplier = 1;
    public float strongAttackDamageMultiplier = 2;
    public int finisherDamageMultiplier = 4;

    [Header("Damage Block")]
    [Range(0,100)]public float physicalDamageBlockPercentage;

    [HideInInspector]public string equipAnimation= "WeaponEquip";

    [Header("Attack Animations")]
    public AttackSet attackSet;
    public List<string> weakAttacks;
    public List<string> strongAttacks;

    [Header("Parry Animation")]
    public string parry= "Parry";

    [Header("Stamina Costs")]
    public int baseStaminaCost;
    public float weakAttackCostMultiplier=1;
    public float strongAttackCostMultiplier=2;

    [Header("Weapon Details")]
    [Tooltip("The type of weapon being weilded, determines how attacks are resolved")]
    public WeaponType weaponType;
    [Tooltip("The slot where the weapon is placed when it is not being weilded")]
    public WeaponSlot weaponSlotWhenNotWielded;

    public bool canParry;
    [Tooltip("Used for when no weapon is equipped")]
    public bool isUnarmed = false;
}

