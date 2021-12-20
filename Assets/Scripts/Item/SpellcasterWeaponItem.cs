using UnityEngine;

[CreateAssetMenu(menuName = "Items/SpellcasterWeapon")]
public class SpellcasterWeaponItem : WeaponItem
{
    [Header("Magic Weapon Values")]
    public SpellType magicWeaponCostType;
    public int magicAttackCost;

    public MagicWeaponAttackData attackData;
}
