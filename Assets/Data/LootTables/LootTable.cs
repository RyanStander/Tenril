using System;
using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

/// <summary>
/// Loot table is defined and meant to serve as the table from which elements like chests and enemies spawn with pre-defined equipment and items
/// </summary>
[CreateAssetMenu(menuName = "LootTable")]
public class LootTable : ScriptableObject
{
    //Defines list of essential items that should always include one
    [Header("Items that are essential to being spawned, but picked between")]
    public LootInfoChanceSeries[] essentialLoot;

    //Defines list of consumable and utility related
    [Header("Consumable and utility based items to populate the inventory")]
    public LootInfoChance[] utilityLoot;

    //Defines list of miscellaneous items
    [Header("Random items to populate the inventory")]
    public LootInfoChance[] miscellaneousLoot;
}
