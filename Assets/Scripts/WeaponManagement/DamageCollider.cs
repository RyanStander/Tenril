using Character;
using UnityEngine;

namespace WeaponManagement
{
    [RequireComponent(typeof(Collider))]
    public class DamageCollider : MonoBehaviour
    {
        private Collider damageCollider;
        [HideInInspector]public CharacterManager characterManager = null;
        public bool enableDamageColliderOnStart = false;

        public float currentDamage = 10;
        [HideInInspector] public WeaponSoundEffects weaponSoundEffects;

        protected bool hasInterrupt=true;
        private void Awake()
        {
            damageCollider = GetComponent<Collider>();
            damageCollider.gameObject.SetActive(true);
            damageCollider.isTrigger = true;
        }

        private void Start()
        {
            damageCollider.enabled = enableDamageColliderOnStart;
        }

        public void EnableDamageCollider(bool hasInterrupt=true)
        {
            this.hasInterrupt = hasInterrupt;

            damageCollider.enabled = true;
        }

        public void DisableDamageCollider()
        {
            damageCollider.enabled = false;
        }

        protected virtual void OnTriggerEnter(Collider other)
        {
            //When the collider enters an character with one of these tags
            //make them take damage
            if (!other.CompareTag("Damageable") && !other.CompareTag("Enemy") && !other.CompareTag("Player")) 
                return;
        
            var characterStats = other.GetComponent<CharacterStats>();
            var targetCharacterManager = other.GetComponent<CharacterManager>();
            var blockingCollider = other.transform.GetComponentInChildren<BlockingCollider>();

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
                //check if the target is blocking
                if (blockingCollider != null && targetCharacterManager.isBlocking)
                {
                    LayerMask blockingLayer = 1 << 15;
                    //Check if a the defender blocking is actually in line
                    if (CharacterUtilityManager.CheckIfHitColliderOnLayer(characterManager.finisherAttackRayCastStartPointTransform.position, targetCharacterManager.characterLockOnPoint.transform.position, blockingLayer))
                    {
                        #region Audio
                        var audioSourceHolder = targetCharacterManager.GetComponentInChildren<AudioSourceHolder>();
                        var targetInventory = targetCharacterManager.GetComponent<CharacterInventory>();

                        if (audioSourceHolder!=null)
                        {
                            if (targetInventory!=null)
                            {
                                if (targetInventory.equippedWeapon!=null)
                                {
                                    if (targetInventory.equippedWeapon.weaponSoundEffects!=null)
                                    {
                                        audioSourceHolder.hitSFX.PlayOneShot(targetInventory.equippedWeapon.weaponSoundEffects.weaponBlockAttack.audioClip);
                                        audioSourceHolder.hitSFX.volume = targetInventory.equippedWeapon.weaponSoundEffects.weaponBlockAttack.volume;
                                    }
                                }
                            }
                        }
                        #endregion

                        var damageAfterBlock = CharacterUtilityManager.CalculateBlockingDamage(currentDamage, blockingCollider.blockingPhysicalDamageAbsorption);
                        characterStats.TakeDamage(damageAfterBlock, true, "BlockGuard");
                        return;
                    }
                }
            }
            
            //check whether characterManager was assigned
            if (characterManager == null)
            {
                //make sure is not hitting self
                if (targetCharacterManager == GetComponentInParent<CharacterManager>()) 
                    return;
                
                TakeNormalDamage(other,characterStats,targetCharacterManager);
            }
            else
            {
                //make sure is not hitting self
                if (targetCharacterManager == characterManager) 
                    return;

                TakeNormalDamage(other,characterStats,targetCharacterManager);
            }
        }

        //Called when nothing the attack goes through normally
        protected virtual void TakeNormalDamage(Collider hitCollider,CharacterStats characterStats, CharacterManager targetCharacterManager)
        {
            characterStats.TakeDamage(currentDamage, hasInterrupt);
            
            if (hitCollider.TryGetComponent(out CharacterEffectsManager characterEffectsManager))
            {
                var contactPoint = hitCollider.ClosestPointOnBounds(transform.position);
                characterEffectsManager.PlayBloodSplatterFx(contactPoint);
            }
            
            #region Audio

            if (targetCharacterManager == null||weaponSoundEffects == null) return;
            
            var audioSourceHolder = targetCharacterManager.GetComponentInChildren<AudioSourceHolder>();

            if (audioSourceHolder == null) 
                return;
            audioSourceHolder.hitSFX.PlayOneShot(weaponSoundEffects.weaponHitFlesh.audioClip);
            audioSourceHolder.hitSFX.volume = weaponSoundEffects.weaponHitFlesh.volume;

            #endregion
        }
    }
}
