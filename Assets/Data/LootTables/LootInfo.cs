using System;
using UnityEngine;

/// <summary>
/// Defines basic data for a given piece of loot
/// </summary>
[Serializable]
public struct LootInfo
{
    //Basic data
    public Item definedItem;
    public Vector2 itemCountRange;
    [Range(0, 100)] public float spawnChance;
}
