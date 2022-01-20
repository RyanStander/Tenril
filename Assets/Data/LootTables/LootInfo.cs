using System;
using UnityEngine;

/// <summary>
/// Defines basic data for a given piece of loot
/// </summary>
[System.Serializable] public class LootInfo : ISerializationCallbackReceiver
{
    //Basic data
    public string lootName;
    public Item definedItem = null;
    public Vector2 itemCountRange;

    public void OnValidate()
    {
        #if UNITY_EDITOR
        if (lootName == "" && definedItem != null)
        {
            lootName = definedItem.itemName;
            Debug.Log("Trying to set name for " + definedItem.itemName);
        }
        #endif
    }

    void ISerializationCallbackReceiver.OnBeforeSerialize() => this.OnValidate();
    void ISerializationCallbackReceiver.OnAfterDeserialize() { }
}

[System.Serializable] public class LootInfoChance : LootInfo
{
    //Basic data
    [Range(0, 100)] public float spawnChance;
}

[System.Serializable] public class LootInfoChanceSeries
{
    //Basic data
    public string seriesName;
    public LootInfoChance[] definedItem;
}