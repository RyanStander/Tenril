using UnityEngine;

[CreateAssetMenu(menuName = "Items/Ammo")]
public class AmmunitionItem : Item
{
    [Header("Ammo Type")]
    public AmmoType ammoType;

    [Header("Ammo Velocity")]
    public float forwardVelocity = 550;
    public float upwardVelocity = 0;
    public float ammoMass = 0;
    public bool useGravity=false;

    [Header("Ammo base damage")]
    public float physicalDamage=50;

    [Header("Item Models")]
    [Tooltip("The model that is displayed in the ranged weapon container")]
    public GameObject loadedItemModel;
    [Tooltip("The model that is displayed that can damage the enemy")]
    public GameObject liveAmmoModel;
    [Tooltip("The model that is instantiated into the collider on contact")]
    public GameObject penetratedModel; 
}
