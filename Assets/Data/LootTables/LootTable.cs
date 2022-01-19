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
    //public List<LootInfo> essentialLoot;

    //Defines list of consumable and utility related
    public List<LootInfo> utilityLoot;

    //Defines list of miscellaneous items
    public List<LootInfo> miscellaneousLoot;

}
