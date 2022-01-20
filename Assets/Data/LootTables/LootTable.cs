using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Loot table is defined and meant to serve as the table from which elements like chests and enemies spawn with pre-defined equipment and items
/// </summary>
[CreateAssetMenu(menuName = "LootTable")]
public class LootTable : ScriptableObject
{
    //Defines list of essential items that should always include one
    [Header("Items that are guaranteed to being spawned, selecting among the series")]
    public LootInfoWeighedSeries[] essentialLoot;

    //Defines list of random chance loot
    [Header("Loot that has a random chance of appearing")]
    public LootInfoWeighed[] randomChanceLoot;
    
    //Helper method for generating and getting a list of loot
    public static List<ItemInventory> GenerateListOfLoot(LootTable givenTable)
    {
        //Declare a temporary list to return
        List<ItemInventory> listOfItems = new List<ItemInventory>();

        //Iterate over essential loot using a "raffle" system
        foreach (LootInfoWeighedSeries lootSeries in givenTable.essentialLoot)
        {
            //Declare a temporary item reference
            ItemInventory lootItem = new ItemInventory();

            #region Raffle Range Calculation
            //Upper bound for selecting a list item
            float upperRangeBound = 0;

            //Iterate over each embedded list and add them ToArray the upper bound
            foreach(LootInfoWeighed loot in lootSeries.weighedSeries)
            {
                upperRangeBound += loot.spawnWeight;
            }

            //Get a random number based on the calculated range
            float randomNumber = UnityEngine.Random.Range(0f, upperRangeBound);
            #endregion

            #region Guaranteed loot selection
            //Iterate over each embedded list and check for spawn chance
            foreach (LootInfoWeighed weighedLoot in lootSeries.weighedSeries)
            {
                //If the random number is less than the spawn chance, continue to next loot series and add item
                if(randomNumber <= weighedLoot.spawnWeight)
                {
                    //Set the item to the weighed loot
                    lootItem.item = weighedLoot.definedItem;

                    //Get a random quantity based on ranges, otherwise use the default given count
                    lootItem.itemStackCount = UnityEngine.Random.Range(weighedLoot.itemCountRange.x, weighedLoot.itemCountRange.y);

                    //Add to the list of items
                    listOfItems.Add(lootItem);

                    //Break out of the loop as an item was found
                    break;
                }
                else
                {
                    //Lower the random number by the spawn chance
                    randomNumber -= weighedLoot.spawnWeight;
                }
            }

            //If no item was added, log an error
            if(lootItem.item == null)
            {
                Debug.LogError("Essential item failed to generate!");
            }
            #endregion
        }

        #region Random chance loot
        //Iterate over random chance loot
        foreach (LootInfoWeighed weighedLoot in givenTable.randomChanceLoot)
        {
            //Compare the chance of spawn against a random number generator
            if (UnityEngine.Random.Range(0f, 100) <= weighedLoot.spawnWeight)
            {
                //Declare a temporary item reference
                ItemInventory lootItem = new ItemInventory();

                //Set the item to the weighed loot
                lootItem.item = weighedLoot.definedItem;

                //Get a random quantity based on ranges, otherwise use the default given count
                lootItem.itemStackCount = UnityEngine.Random.Range(weighedLoot.itemCountRange.x, weighedLoot.itemCountRange.y);

                //Add the item to the list
                listOfItems.Add(lootItem);
            }
        }
        #endregion

        return listOfItems;
    }
}
