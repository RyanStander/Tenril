using UnityEngine;

/// <summary>
/// Defines basic data for a given piece of loot
/// </summary>
[System.Serializable] public class LootInfo : ISerializationCallbackReceiver
{
    //Basic data
    public string lootName;
    public Item definedItem = null;
    public Vector2Int itemCountRange = new Vector2Int(1, 1);

    public void OnValidate()
    {
        #if UNITY_EDITOR
        if (lootName == "" && definedItem != null)
        {
            lootName = definedItem.itemName;
            Debug.Log("Loot table item automatically named to: " + definedItem.itemName);
        }
        #endif
    }

    //Allows for OnValidate to function without being a monobehaviour
    void ISerializationCallbackReceiver.OnBeforeSerialize() => this.OnValidate();
    void ISerializationCallbackReceiver.OnAfterDeserialize() { }
}

[System.Serializable] public class LootInfoWeighed : LootInfo
{
    //Basic data
    [Range(0, 100)] public float spawnWeight;
}

[System.Serializable] public class LootInfoWeighedSeries
{
    //Basic data
    public string seriesName;
    public LootInfoWeighed[] weighedSeries;
}