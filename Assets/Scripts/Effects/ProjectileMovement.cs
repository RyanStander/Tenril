using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Collider))]
[RequireComponent(typeof(Rigidbody))]
public class ProjectileMovement : MonoBehaviour
{
    public float projectileSpeed=0.5f;
    void FixedUpdate()
    {
        transform.position += transform.forward* projectileSpeed;
    }

    private void OnTriggerEnter(Collider other)
    {
        CharacterManager characterManager = (GetComponent(typeof(DamageCollider)) as DamageCollider).characterManager;
        if (other.GetComponent<CharacterStats>() == characterManager)
            return;

        Destroy(gameObject);
    }
}
