using UnityEngine;

[CreateAssetMenu(menuName = "Misc/MagicWeaponAttacks")]
public class MagicWeaponAttackData : ScriptableObject
{
    [Header("projectile Velocity")]
    public float forwardVelocity = 550;
    public float upwardVelocity = 0;
    public float projectileMass = 0;
    public bool useGravity = false;

    [Header("projectile base damage")]
    public float physicalDamage = 50;

    [Header("Item Models")]
    [Tooltip("The model that is displayed when casting with the weapon")]
    public GameObject castingModel;
    [Tooltip("The model that is displayed that can damage the enemy")]
    public GameObject liveProjectileModel;
}
