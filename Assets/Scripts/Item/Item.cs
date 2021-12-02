using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;


public class Item : ScriptableObject
{
    [Header("Item Information")]
    public string UID;
    public Sprite itemIcon;
    public string itemName;
    [Tooltip("The maximum amount of the item that can be carried")]
    public int amountPerStack=1;
    public int ItemValue;

    //Create a UID if it does not already exist
    private void OnValidate()
    {
    #if UNITY_EDITOR
        if (UID == "")
        {   
            UID = GUID.Generate().ToString();
            UnityEditor.EditorUtility.SetDirty(this);
        }
    #endif
    }
}