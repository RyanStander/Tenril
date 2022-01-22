using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DamagePlayer : MonoBehaviour
{
    [SerializeField] private float damage = 10;
    private void OnTriggerEnter(Collider other)
    {
        //Simple script to do damage when player enters a collider, used for testing mainly
        PlayerStats playerStats = other.GetComponent<PlayerStats>();

        if (playerStats != null)
        {
            playerStats.TakeDamage(damage);
        }
    }
}