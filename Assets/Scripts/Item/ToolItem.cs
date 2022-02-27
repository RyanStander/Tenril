using UnityEngine;

[CreateAssetMenu(menuName = "Items/Tool")]
/// <summary>
/// Holds the data of tools, tools are used to harvest or interact with in world objects
/// </summary>
public class ToolItem : Item
{
    public GameObject toolPrefab;
    public ToolType toolType;
}
