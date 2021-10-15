using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "Status Effects/Basic Status Effect")]
public class StatusEffect : ScriptableObject
{
    public StatusEffectType statusEffectType;
    public float statusEffectDuration;
    public Sprite itemIcon;
}

public enum StatusEffectType
{
    none,
    poisoned,
    rooted,
    burning,
    frozen
}
