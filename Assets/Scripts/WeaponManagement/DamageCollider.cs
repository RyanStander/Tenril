using UnityEngine;

[RequireComponent(typeof(Collider))]
public class DamageCollider : MonoBehaviour
{
    private Collider damageCollider;
    [HideInInspector]public CharacterManager characterManager = null;

    public float currentDamage = 10;

    private bool hasInterrupt=true;
    private void Awake()
    {
        damageCollider = GetComponent<Collider>();
        damageCollider.gameObject.SetActive(true);
        damageCollider.isTrigger = true;
        damageCollider.enabled = false;
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

    private void OnTriggerEnter(Collider other)
    {
        //When the collider enters an character with one of these tags
        //make them take damage
        if (other.CompareTag("Damageable") || other.CompareTag("Enemy") || other.CompareTag("Player"))
        {
            CharacterStats characterStats = other.GetComponent<CharacterStats>();
            CharacterManager targetCharacterManager = other.GetComponent<CharacterManager>();
            BlockingCollider blockingCollider = other.transform.GetComponentInChildren<BlockingCollider>();

            if (targetCharacterManager!=null)
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
                    float damageAfterBlock = CharacterUtilityManager.CalculateBlockingDamage(currentDamage, blockingCollider.blockingPhysicalDamageAbsorption);
                    if (characterStats != null)
                    {
                        characterStats.TakeDamage(damageAfterBlock, true, "BlockGuard");
                        return;
                    }
                }
            }

            if (characterStats == null)
                return;
            
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
                if(targetCharacterManager != characterManager)
                    characterStats.TakeDamage(currentDamage, hasInterrupt);
            }

        }
    }
}
