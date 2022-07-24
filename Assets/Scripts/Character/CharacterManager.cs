using UnityEngine;
using WeaponManagement;

namespace Character
{
    public class CharacterManager : MonoBehaviour
    {
        public CharacterLockOnPoint characterLockOnPoint;
        public WeaponSlotManager weaponSlotManager;
        protected RagdollManager ragdollManager;
        public Rigidbody rigidBody;
    
        public bool isParrying, canBeRiposted, isBlocking;

        [Header("Finishers Data")]
        [Tooltip("Placed on the character to indicate where the riposte can be performed from, this should be on the front")]
        public FinisherDamageCollider riposteCollider;
        //Damage will be inflicted during an animation event, used in backstab or riposte animations
        [Tooltip("The damage dealt during backstabs/counters")] 
        public float pendingFinisherDamage;
        [Tooltip("Finisher raycast position (raycasts a line out to check if it hits any finisherDamagerColliders) This should be placed a bit in front of the character's chest.")] 
        public Transform finisherAttackRayCastStartPointTransform;
        //The value used for determinging how the magicka is regenerated
        protected float timeStrength;

        public virtual void EnableRagdoll()
        {
            if (ragdollManager != null)
            {
                ragdollManager.EnableRagdollComponents();
            }
            GetComponent<Rigidbody>().useGravity = false;

            if (weaponSlotManager!=null)
            {
                weaponSlotManager.CloseDamageCollider();
            }
        }
    }
}
