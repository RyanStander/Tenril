using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "Sounds/CharacterSoundEffects")]
public class CharacterSoundEffects : ScriptableObject
{
    [Header("Voices")]
    public List<AudioClipData> gruntVoices;
    public List<AudioClipData> hurtVoices;
    public List<AudioClipData> dieVoices;
    public List<AudioClipData> jumpVoices;
    public List<AudioClipData> landVoices;
    [Header("SFX")]
    public WalkingSFX leftFootstepSFX;
    public WalkingSFX rightFootstepSFX;
    public AudioClipData jumpSFX,landSFX,dodgeSFX,rollSFX;
}

[System.Serializable]
public class WalkingSFX
{
    [Tooltip("The audio clip to be played")]
    public AudioClip audioClip;
    [Tooltip("The volume level based on how fast the character is running")]
    [Range(0,1)]public float walkingVolume=0.5f,runningVolume=0.75f,sprintingVolume=1f;
}
