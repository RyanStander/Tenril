using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DamageCharacter : MonoBehaviour
{
    [SerializeField] private float damage = 10;
    private void OnTriggerEnter(Collider other)
    {
        //Simple script to do damage when a character enters a collider, used for testing mainly
        CharacterStats characterStats = other.GetComponent<CharacterStats>();

        if (characterStats != null)
        {
            characterStats.TakeDamage(damage);
        }
    }
}
