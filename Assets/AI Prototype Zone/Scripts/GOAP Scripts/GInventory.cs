using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GInventory : MonoBehaviour
{
    //Item references saved to the inventory
    public List<GameObject> items = new List<GameObject>();

    //Treasure backpacks
    public int totalTreasure = 0;

    //[Range(1,100)]
    public int treasureCapacity = 10;

    //Add an item
    public void Additem(GameObject item)
    {
        items.Add(item);
    }

    //Find an inventory stored object using a tag
    public GameObject FindItemWithTag(string tag)
    {
        foreach(GameObject item in items)
        {
            if(item.tag == tag)
            {
                return item;
            }
        }
        return null;
    }

    //Remove an item
    public void RemoveItem(GameObject item)
    {
        int indexToRemove = -1;
        foreach(GameObject gameObject in items)
        {
            indexToRemove++;
            if(gameObject == item)
            {
                break;
            }
        }
        if(indexToRemove >= -1)
        {
            items.RemoveAt(indexToRemove);
        }
    }

    //Check if capacity is reached of a specific resource
    public bool HasReachedCapacity(int current, int capacity)
    {
        //If at capacity, return true
        if (current >= capacity)
        {
            return true;
        }
        else
        {
            return false;
        }
    }

    //Returns the excess amount of treasure that cant be added after adding
    public int AddTreasure(int amountToAdd)
    {
        //Add the amount
        totalTreasure += amountToAdd;

        //Excess to calculate
        int excess = 0;

        //Calculate excess if over treasure capacity
        if (totalTreasure > treasureCapacity)
        {
            excess = totalTreasure - treasureCapacity;
        }

        //Clamp the treasure
        totalTreasure = Mathf.Clamp(totalTreasure, 0, treasureCapacity);

        //Return excess
        return excess;
    }

    public void ClearTreasure()
    {
        totalTreasure = 0;
    }
}
