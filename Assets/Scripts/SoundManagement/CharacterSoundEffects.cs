using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "Sounds/CharacterSoundEffects")]
public class CharacterSoundEffects : ScriptableObject
{
    [SerializeField] public WalkingSFX leftFootstepSFX, rightFootstepSFX;
    [SerializeField] public AudioClip jumpSFX,landSFX;
}

[System.Serializable]
public class WalkingSFX
{
    [Tooltip("The audio clip to be played")]
    public AudioClip audioClip;
    [Tooltip("The volume level based on how fast the character is running")]
    [Range(0,1)]public float walkingVolume=0.5f,runningVolume=0.75f,sprintingVolume=1f;
}
