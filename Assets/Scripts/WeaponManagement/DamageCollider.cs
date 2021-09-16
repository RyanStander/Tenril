using UnityEngine;

[RequireComponent(typeof(Collider))]
public class DamageCollider : MonoBehaviour
{
    private Collider damageCollider;

    public float currentWeaponDamage = 10;
    private void Awake()
    {
        damageCollider = GetComponent<Collider>();
        damageCollider.gameObject.SetActive(true);
        damageCollider.isTrigger = true;
        damageCollider.enabled = false;
    }

    public void EnableDamageCollider()
    {
        damageCollider.enabled = true;
    }

    public void DisableDamageCollider()
    {
        damageCollider.enabled = false;
    }

    private void OnTriggerEnter(Collider other)
    {
        Debug.Log("Hit Object");
        //When the collider enters an character with one of these tags
        //make them take damage
        if (other.CompareTag("Damageable")|| other.CompareTag("Enemy")|| other.CompareTag("Player"))
        {
            Debug.Log("Hit Object");

            CharacterStats characterStats = other.GetComponent<CharacterStats>();

            if (characterStats != null)
            {
                characterStats.TakeDamage(currentWeaponDamage,true);
            }
        }

        /*if (other.CompareTag("Damageable"))
        {
        }
        if (other.CompareTag("Enemy"))
        {
        }
        if (other.CompareTag("Player"))
        {
        }*/
    }
}
