using System.Collections.Generic;
using UnityEngine;

public class CharacterSoundManager : MonoBehaviour
{
    private AudioSourceHolder audioSourceHolder;
    private CharacterInventory characterInventory;
    private CharacterSpellcastingManager spellcastingManager;
    private Animator animator;
    [SerializeField] private CharacterSoundEffects characterSoundEffectSet;
    [SerializeField] private ToolsSoundEffects toolsSoundEffectSet;

    private float forwardValueLimitLeftFootstepSounds = 0.4f;

    /// <summary>
    /// The sound manager that handles footsteps.
    /// </summary>
    private FootstepTextureSoundManager footstepTextureSoundManager;

    private void Start()
    {
        audioSourceHolder = GetComponentInChildren<AudioSourceHolder>();
        animator = GetComponent<Animator>();
        characterInventory = GetComponent<CharacterInventory>();
        spellcastingManager = GetComponent<CharacterSpellcastingManager>();
        footstepTextureSoundManager = GetComponent<FootstepTextureSoundManager>();
    }

    #region Voice

    private void GruntVoiceSound()
    {
        if (!HasSetCharacterSoundEffects())
            return;

        int index = Random.Range(0, characterSoundEffectSet.gruntVoices.Count);
        PlayVoiceClip(characterSoundEffectSet.gruntVoices[index].audioClip, characterSoundEffectSet.gruntVoices[index].volume);
    }

    private void HurtVoiceSound()
    {
        if (!HasSetCharacterSoundEffects())
            return;

        int index = Random.Range(0, characterSoundEffectSet.hurtVoices.Count);
        PlayVoiceClip(characterSoundEffectSet.hurtVoices[index].audioClip, characterSoundEffectSet.hurtVoices[index].volume);
    }

    private void DieVoiceSound()
    {
        if (!HasSetCharacterSoundEffects())
            return;

        int index = Random.Range(0, characterSoundEffectSet.dieVoices.Count);
        PlayVoiceClip(characterSoundEffectSet.dieVoices[index].audioClip, characterSoundEffectSet.dieVoices[index].volume);
    }

    private void JumpVoiceSound()
    {
        if (!HasSetCharacterSoundEffects())
            return;

        int index = Random.Range(0, characterSoundEffectSet.jumpVoices.Count);
        PlayVoiceClip(characterSoundEffectSet.jumpVoices[index].audioClip, characterSoundEffectSet.jumpVoices[index].volume);
    }

    private void LandVoiceSound()
    {
        if (!HasSetCharacterSoundEffects())
            return;

        int index = Random.Range(0, characterSoundEffectSet.landVoices.Count);
        PlayVoiceClip(characterSoundEffectSet.landVoices[index].audioClip, characterSoundEffectSet.landVoices[index].volume);
    }

    private void PlayVoiceClip(AudioClip audioClip, float volume)
    {
        audioSourceHolder.voiceSFX.PlayOneShot(audioClip);
        audioSourceHolder.voiceSFX.volume = volume;
    }

    #endregion

    #region SFX

    #region locomotion
    private void ForwardFootstepL(float forward)
    {
        if (isInteracting())
            return;

        if (!HasSetCharacterSoundEffects())
            return;

        float actualForward = animator.GetFloat("Forward");

        // If character sprinting forward.
        if (forward==-2f || forward== 2f)
        {
            PlayFootstepSoundClip(footstepTextureSoundManager.GetFootstepTextureSound(characterSoundEffectSet.footstepSFX.SprintingSounds));
        }
        // If character running forward.
        else if ((forward==1f&&actualForward<= 1f) ||(forward==-1f && actualForward >= -1f))
        {
            PlayFootstepSoundClip(footstepTextureSoundManager.GetFootstepTextureSound(characterSoundEffectSet.footstepSFX.RunningSounds));
        }
        // If character walking forward.
        else if ((forward == 0.5f &&actualForward<=0.5f) || (forward == -0.5f && actualForward >= -0.5f))
        {
            PlayFootstepSoundClip(footstepTextureSoundManager.GetFootstepTextureSound(characterSoundEffectSet.footstepSFX.WalkingSounds));
        }
    }

    private void LeftFootstepL(float left)
    {
        if (isInteracting())
            return;

        if (!HasSetCharacterSoundEffects())
            return;

        float actualLeft = animator.GetFloat("Left");
        float actualForward = animator.GetFloat("Forward");

        if (!CanPlayLeftFootstep(actualForward))
        {
            return;
        }
        // If character running left.
        if ((left == 1 && actualLeft <= 1)||(left==-1&&actualLeft>=-1))
        {
            PlayFootstepSoundClip(footstepTextureSoundManager.GetFootstepTextureSound(characterSoundEffectSet.footstepSFX.RunningSounds));
        }
        // If character walking left.
        else if ((left == 0.5f && actualLeft <= 0.5f)|| (left == -0.5f && actualLeft >= -0.5f))
        {
            PlayFootstepSoundClip(footstepTextureSoundManager.GetFootstepTextureSound(characterSoundEffectSet.footstepSFX.WalkingSounds));
        }
    }

    private void ForwardFootstepR(float forward)
    {
        if (isInteracting())
            return;

        if (!HasSetCharacterSoundEffects())
            return;

        float actualForward = animator.GetFloat("Forward");

        // If character sprinting forward.
        if (forward == -2 || forward == 2)
        {
            PlayFootstepSoundClip(footstepTextureSoundManager.GetFootstepTextureSound(characterSoundEffectSet.footstepSFX.SprintingSounds));
        }
        // If character running forward.
        else if ((forward == 1 && actualForward <= 1) || (forward == -1 && actualForward >= -1))
        {
            PlayFootstepSoundClip(footstepTextureSoundManager.GetFootstepTextureSound(characterSoundEffectSet.footstepSFX.RunningSounds));
        }
        // If character walking forward.
        else if ((forward == 0.5f && actualForward <= 0.5f) || (forward == -0.5f && actualForward >= -0.5f))
        {
            PlayFootstepSoundClip(footstepTextureSoundManager.GetFootstepTextureSound(characterSoundEffectSet.footstepSFX.WalkingSounds));
        }
    }

    private void LeftFootstepR(float left)
    {
        if (isInteracting())
            return;

        if (!HasSetCharacterSoundEffects())
            return;

        float actualLeft = animator.GetFloat("Left");
        float actualForward = animator.GetFloat("Forward");

        if (!CanPlayLeftFootstep(actualForward))
        {
            return;
        }
        // If character running left.
        if ((left == 1 && actualLeft <= 1) || (left == -1 && actualLeft >= -1))
        {
            PlayFootstepSoundClip(footstepTextureSoundManager.GetFootstepTextureSound(characterSoundEffectSet.footstepSFX.RunningSounds));
        }
        // If character walking left.
        else if ((left == 0.5f && actualLeft <= 0.5f) || (left == -0.5f && actualLeft >= -0.5f))
        {
            PlayFootstepSoundClip(footstepTextureSoundManager.GetFootstepTextureSound(characterSoundEffectSet.footstepSFX.WalkingSounds));
        }
    }

    /// <summary>
    /// Functions as a way to have footsteps ignore any restrictions on movements speed, used for animations that have walking but doesnt use forward/left movement values
    /// </summary>
    private void ForcedFootsetpL()
    {
        if (!HasSetCharacterSoundEffects())
            return;

        PlayFootstepSoundClip(footstepTextureSoundManager.GetFootstepTextureSound(characterSoundEffectSet.footstepSFX.RunningSounds));
    }

    /// <summary>
    /// Functions as a way to have footsteps ignore any restrictions on movements speed, used for animations that have walking but doesnt use forward/left movement values
    /// </summary>
    private void ForcedFootsetpR()
    {
        if (!HasSetCharacterSoundEffects())
            return;

        PlayFootstepSoundClip(footstepTextureSoundManager.GetFootstepTextureSound(characterSoundEffectSet.footstepSFX.RunningSounds));
    }

    private void RollSFX()
    {
        if (!HasSetCharacterSoundEffects())
            return;

        PlaySoundClip(characterSoundEffectSet.dodgeSFX.audioClip, characterSoundEffectSet.dodgeSFX.volume);
    }

    private void LandSFX()
    {
        if (!HasSetCharacterSoundEffects())
            return;

        PlaySoundClip(characterSoundEffectSet.landSFX.audioClip, characterSoundEffectSet.landSFX.volume);
    }

    /// <summary>
    /// Plays a given audio clip at a given volume.
    /// </summary>
    /// <param name="audioClip">The audioclip to play.</param>
    /// <param name="volume">The volume to play the clip at.</param>
    private void PlaySoundClip(AudioClip audioClip, float volume)
    {
        // Nullcheck for safety.
        if(audioClip == null)
        {
            Debug.LogWarning("Tried to play an audio clip, but it was null!");
            return;
        }

        audioSourceHolder.locomotionSFX.PlayOneShot(audioClip);
        audioSourceHolder.locomotionSFX.volume = volume;
    }

    /// <summary>
    /// Plays a given audio clip pair at a given volume.
    /// </summary>
    /// <param name="pair">The audioclip and volume pair to play.</param>
    private void PlaySoundClip(ClipVolumePair pair) => PlaySoundClip(pair.audioClip, pair.audioVolume);

    /// <summary>
    /// Plays a given list of footstep sound clip pairs.
    /// </summary>
    /// <param name="pairs">List of audioclip and volume pairs to play.</param>
    private void PlayFootstepSoundClip(List<ClipVolumePair> pairs)
    {
        // Check if no valid clips were found.
        if(pairs.Count == 0 || pairs[0].audioClip == null)
        {
            PlaySoundClip(characterSoundEffectSet.footstepSFX.UndefinedFootstepsSounds.GetUniqueRandomClipPair());
            return;
        }

        // Play all the clips.
        foreach (ClipVolumePair pair in pairs)
        {
            PlaySoundClip(pair);
        }
    }
    #endregion

    #region Weapon
    private void DrawWeaponSound()
    {
        if (!CheckIfWeaponSFXIsSet())
            return;

        PlaySoundClip(characterInventory.equippedWeapon.weaponSoundEffects.weaponDrawSFX.audioClip, characterInventory.equippedWeapon.weaponSoundEffects.weaponDrawSFX.volume);
    }

    private void WeaponSwingSound()
    {
        if (!CheckIfWeaponSFXIsSet())
            return;

        PlaySoundClip(characterInventory.equippedWeapon.weaponSoundEffects.weaponSwingSFX.audioClip, characterInventory.equippedWeapon.weaponSoundEffects.weaponSwingSFX.volume);
    }

    private void WeaponHitSound()
    {
        if (!CheckIfWeaponSFXIsSet())
            return;

        PlaySoundClip(characterInventory.equippedWeapon.weaponSoundEffects.weaponHitFlesh.audioClip, characterInventory.equippedWeapon.weaponSoundEffects.weaponSwingSFX.volume);
    }

    private void RangedLoadSound()
    {
        if (!CheckIfWeaponSFXIsSet())
            return;

        if (characterInventory.equippedWeapon.weaponSoundEffects is RangedWeaponSoundEffects rangedWeaponSoundEffects)
        {
            PlaySoundClip(rangedWeaponSoundEffects.loadSFX.audioClip, rangedWeaponSoundEffects.loadSFX.volume);
        }
    }

    private void RangedFireSound()
    {
        if (!CheckIfWeaponSFXIsSet())
            return;

        if (characterInventory.equippedWeapon.weaponSoundEffects is RangedWeaponSoundEffects rangedWeaponSoundEffects)
        {
            PlaySoundClip(rangedWeaponSoundEffects.fireSFX.audioClip, rangedWeaponSoundEffects.fireSFX.volume);
        }
    }
    #endregion

    #region Spellcasting

    private void CastSpellSFX()
    {
        if (!CheckIfSpellSFXIsSet())
            return;

        if (spellcastingManager.spellBeingCast.castSFX.audioClip == null)
            return;

        PlaySpellSoundClip(spellcastingManager.spellBeingCast.castSFX.audioClip, spellcastingManager.spellBeingCast.castSFX.volume);
    }

    private void WindUpSpellSFX()
    {
        if (!CheckIfSpellSFXIsSet())
            return;

        if (spellcastingManager.spellBeingCast.windUPSFX.audioClip == null)
            return;

        PlaySpellSoundClip(spellcastingManager.spellBeingCast.windUPSFX.audioClip, spellcastingManager.spellBeingCast.windUPSFX.volume);
    }

    private void PlaySpellSoundClip(AudioClip audioClip, float volume)
    {
        audioSourceHolder.spellcastSFX.PlayOneShot(audioClip);
        audioSourceHolder.spellcastSFX.volume = volume;
    }

    #endregion

    #region Tools

    private void MineSound()
    {
        if (!CheckIfToolsSFXIsSet())
            return;

        PlayVoiceClip(toolsSoundEffectSet.miningSFX.audioClip, toolsSoundEffectSet.miningSFX.volume);
    }

    private void ChopSound()
    {
        if (!CheckIfToolsSFXIsSet())
            return;

        PlayVoiceClip(toolsSoundEffectSet.choppingSFX.audioClip, toolsSoundEffectSet.choppingSFX.volume);
    }

    #endregion

    #endregion

    #region Checkers
    private bool isInteracting()
    {
        return animator.GetBool("isInteracting");
    }
    private bool CanPlayLeftFootstep(float actualForward)
    {
        if (!(actualForward <= forwardValueLimitLeftFootstepSounds && actualForward >= -forwardValueLimitLeftFootstepSounds))
        {
            return false;
        }
        else
        {
            return true;
        }
    }

    private bool HasSetCharacterSoundEffects()
    {
        if (characterSoundEffectSet == null)
        {
            Debug.LogWarning("no sound effect set for the character, make sure you have set it");
            return false;
        }
        else
        {
            return true;
        }
    }

    private bool CheckIfWeaponSFXIsSet()
    {
        if (characterInventory == null)
        {
            Debug.LogWarning("Could not find character's inventroy");
            return false;
        }
        else if (characterInventory.equippedWeapon==null)
        {
            Debug.LogWarning("Could not find equipped weapon");
            return false;
        }
        else if (characterInventory.equippedWeapon.weaponSoundEffects==null)
        {
            Debug.LogWarning("Could not find weaponSoundEffects on equipped weapon");
            return false;
        }
        else
        {
            return true;
        }
    }

    private bool CheckIfSpellSFXIsSet()
    {
        if (spellcastingManager == null)
        {
            Debug.LogWarning("Could not find character's spellcasting Manager");
            return false;
        }
        else if (spellcastingManager.spellBeingCast == null)
        {
            Debug.LogWarning("Could not find equipped weapon");
            return false;
        }
        else
        {
            return true;
        }
    }

    private bool CheckIfToolsSFXIsSet()
    {
        if (characterSoundEffectSet == null)
        {
            Debug.LogWarning("no tools sound effect set for the character, make sure you have set it");
            return false;
        }
        else
        {
            return true;
        }
    }
    #endregion
}
