using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "Items/Outfit")]

public class OutfitItem : Item
{
    [Header("Stat Bonuses")]
    public float healthEnhacement;
    public float staminaEnhancement;
    public float moonlightEnhancement;
    public float sunlightEnhancement;
    public float damageEnhancement;
    [Header("Outfit Properties")]
    public GameObject outfitPrefab;
    public Faction outfitFaction;
    [TextArea] public string outfitDetails;
    [Tooltip("A special feature that is only available while you have the outfit equipped")]
    public SpellItem specialAbility;
}
