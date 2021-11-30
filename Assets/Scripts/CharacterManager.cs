using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CharacterManager : MonoBehaviour
{
    public Transform lockOnTransform;
    protected WeaponSlotManager weaponSlotManager;
    
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
}
