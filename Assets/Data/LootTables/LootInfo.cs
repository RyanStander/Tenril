using System;
using UnityEngine;

/// <summary>
/// Defines basic data for a given piece of loot
/// </summary>
[System.Serializable] public class LootInfo
{
    //Basic data
    public Item definedItem;
    public Vector2 itemCountRange;
}

[System.Serializable] public class LootInfoChance : LootInfo
{
    //Basic data
    [Range(0, 100)] public float spawnChance;
}

[System.Serializable] public class LootInfoSeries
{
    //Basic data
    public LootInfo[] definedItem;
}

[System.Serializable] public class LootInfoChanceSeries
{
    //Basic data
    public LootInfoChance[] definedItem;
}