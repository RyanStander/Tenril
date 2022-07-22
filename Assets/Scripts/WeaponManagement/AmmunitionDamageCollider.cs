using Character;
using UnityEngine;
public class AmmunitionDamageCollider : ProjectileDamageCollider
{
    public AmmunitionItem ammoItem;
    protected bool hasAlreadyPenetratedASurface;
    [SerializeField] private LayerMask ignoreLayers;

    protected override void OnTriggerEnter(Collider other)
    {
        base.OnTriggerEnter(other);
        if (!hasAlreadyPenetratedASurface)
        {
            if (!(ignoreLayers == (ignoreLayers | (1 << other.gameObject.layer))))
            {

                if (characterManager == other.GetComponentInParent<CharacterManager>())
                    return;

                //Debug.Log(other.gameObject.layer + other.name);
                //create a projectile
                GameObject penetratedProjectile = Instantiate(ammoItem.penetratedModel, transform.position - transform.forward, transform.rotation);
                penetratedProjectile.transform.parent = other.transform;

                hasAlreadyPenetratedASurface = true;
                Destroy(transform.root.gameObject);
            }
        }
    }
}
