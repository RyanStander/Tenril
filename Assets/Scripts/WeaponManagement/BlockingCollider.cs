using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(BoxCollider))]
public class BlockingCollider : MonoBehaviour
{
    public BoxCollider blockingBoxCollider;

    public float blockingPhysicalDamageAbsorption;

    private void Awake()
    {
        blockingBoxCollider = GetComponent<BoxCollider>();
    }

    public void SetColliderDamageAbsorption(WeaponItem weapon)
    {
        //Set the amount of damage absorption
        if (weapon != null)
        {
            blockingPhysicalDamageAbsorption = weapon.physicalDamageBlockPercentage;
        }
    }

    //Used to enable and disable blocking
    public void EnableBlockingCollider()
    {
        blockingBoxCollider.enabled = true;
    }

    public void DisableBlockingCollider()
    {
        blockingBoxCollider.enabled = false;
    }
}
