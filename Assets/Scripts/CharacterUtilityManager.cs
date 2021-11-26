
using UnityEngine;
/// <summary>
/// The CUM serves as a helper class that holds methods for utility. Let the CUM enter your scripts.
/// </summary>
public static class CharacterUtilityManager
{
    /// <summary>
    /// Returns the amount of damage that a character would take depending on its resistances and the attackers damage
    /// whilst blocking.
    /// </summary>
    public static float CalculateBlockingDamage(float currentWeaponDamage, float blockingPhysicalDamageAbsorption)
    {
        return currentWeaponDamage - (currentWeaponDamage * blockingPhysicalDamageAbsorption) / 100;
        //TODO: This currently only uses weapon damage and blocking damage absorption, for future needs resistances and damage bonuses taken into account
    }


    public static bool CheckIfHitColliderOnLayer(Vector3 attackerPosition, Vector3 defenderPosition, LayerMask layerToCheck)
    {
        //calculate the direction and distance between the attacker and defender
        Vector3 direction = defenderPosition - attackerPosition;
        float distance =Vector3.Distance(attackerPosition, defenderPosition);
        RaycastHit hit;

        if (Physics.Raycast(attackerPosition, direction, out hit, distance, layerToCheck))
        {
            if (hit.transform != null)
                return true;
        }
        return false;
    }
}
