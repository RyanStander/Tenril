
using UnityEngine;

public class RagdollManager : MonoBehaviour
{
    private void Start()
    {
        setRigidbodyState(true);
        setColliderState(false);
    }
    private void setRigidbodyState(bool state)
    {
        Rigidbody[] rigidbodies = GetComponentsInChildren<Rigidbody>();

        foreach (Rigidbody rigidbody in rigidbodies)
        {
            rigidbody.isKinematic = state;
            rigidbody.velocity = Vector3.zero;
        }
    }

    private void setColliderState(bool state)
    {
        Collider[] colliders = GetComponentsInChildren<Collider>();

        foreach (Collider collider in colliders)
        {
            collider.enabled = state;
        }
    }

    internal void EnableRagdollComponents()
    {
        setRigidbodyState(false);
        setColliderState(true);
    }
}
