using Character;
using UnityEngine;
using WeaponManagement;

public class ProjectileDamageCollider : DamageCollider
{
    protected override void OnTriggerEnter(Collider other)
    {
        //When the collider enters an character with one of these tags
        //make them take damage
        if (other.CompareTag("Damageable") || other.CompareTag("Enemy") || other.CompareTag("Player"))
        {
            CharacterStats characterStats = other.GetComponent<CharacterStats>();
            CharacterManager targetCharacterManager = other.GetComponent<CharacterManager>();
            BlockingCollider blockingCollider = other.transform.GetComponentInChildren<BlockingCollider>();

            if (characterStats == null)
                return;

            if (targetCharacterManager != null)
            {
                //check if the target is parrying
                if (targetCharacterManager.isParrying)
                {
                    //possibly make check in future for certain attacks that arent parryable
                    if (characterManager != null)
                        characterManager.GetComponent<AnimatorManager>().PlayTargetAnimation("Parried", true);
                    else
                        Debug.LogWarning("characterManager for damage collider was not set, cant do parries without it, please set it");

                    return;
                }
                else if (blockingCollider != null && targetCharacterManager.isBlocking)
                {
                    LayerMask blockingLayer = 1 << 15;
                    //Check if a the defender blocking is actually in line
                    if (CharacterUtilityManager.CheckIfHitColliderOnLayer(characterManager.finisherAttackRayCastStartPointTransform.position, targetCharacterManager.characterLockOnPoint.transform.position, blockingLayer))
                    {
                        float damageAfterBlock = CharacterUtilityManager.CalculateBlockingDamage(currentDamage, blockingCollider.blockingPhysicalDamageAbsorption);
                        characterStats.TakeDamage(damageAfterBlock, true, "BlockGuard");
                        return;
                    }
                }
            }

            //check whether characterManager was assigned
            if (characterManager == null)
            {
                //make sure is not hitting self
                if (targetCharacterManager != GetComponentInParent<CharacterManager>())
                    characterStats.TakeDamage(currentDamage, hasInterrupt);
            }
            else
            {
                //make sure is not hitting self
                if (targetCharacterManager != characterManager)
                    characterStats.TakeDamage(currentDamage, hasInterrupt);
            }
        }
    }
}
