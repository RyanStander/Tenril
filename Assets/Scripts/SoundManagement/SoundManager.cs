using UnityEngine;

public class SoundManager : MonoBehaviour
{
    private AudioSourceHolder audioSourceHolder;
    private CharacterInventory characterInventory;
    private Animator animator;
    [SerializeField] private CharacterSoundEffects characterSoundEffectSet;

    private float forwardValueLimitLeftFootstepSounds = 0.4f;
    private void Start()
    {
        audioSourceHolder = GetComponentInChildren<AudioSourceHolder>();
        animator = GetComponent<Animator>();
        characterInventory = GetComponent<CharacterInventory>();
    }

    #region locomotion
    private void ForwardFootstepL(float forward)
    {
        if (isInteracting())
            return;

        if (!HasSetCharacterSoundEffects())
            return;

        float actualForward = animator.GetFloat("Forward");

        //if character sprinting forward
        if (forward==-2f || forward== 2f)
        {
            PlaySoundClip(characterSoundEffectSet.leftFootstepSFX.audioClip, characterSoundEffectSet.leftFootstepSFX.sprintingVolume);
        }
        //if character running forward
        else if ((forward==1f&&actualForward<= 1f) ||(forward==-1f && actualForward >= -1f))
        {
            PlaySoundClip(characterSoundEffectSet.leftFootstepSFX.audioClip, characterSoundEffectSet.leftFootstepSFX.runningVolume);
        }
        //if character walking forward
        else if ((forward == 0.5f &&actualForward<=0.5f) || (forward == -0.5f && actualForward >= -0.5f))
        {
            PlaySoundClip(characterSoundEffectSet.leftFootstepSFX.audioClip, characterSoundEffectSet.leftFootstepSFX.walkingVolume);
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
        //if character running left
        if ((left == 1 && actualLeft <= 1)||(left==-1&&actualLeft>=-1))
        {
            PlaySoundClip(characterSoundEffectSet.leftFootstepSFX.audioClip, characterSoundEffectSet.leftFootstepSFX.runningVolume);
        }
        //if character walking left
        else if ((left == 0.5f && actualLeft <= 0.5f)|| (left == -0.5f && actualLeft >= -0.5f))
        {
            PlaySoundClip(characterSoundEffectSet.leftFootstepSFX.audioClip, characterSoundEffectSet.leftFootstepSFX.walkingVolume);
        }
    }

    private void ForwardFootstepR(float forward)
    {
        if (isInteracting())
            return;

        if (!HasSetCharacterSoundEffects())
            return;

        float actualForward = animator.GetFloat("Forward");

        //if character sprinting forward
        if (forward == -2 || forward == 2)
        {
            PlaySoundClip(characterSoundEffectSet.rightFootstepSFX.audioClip, characterSoundEffectSet.rightFootstepSFX.sprintingVolume);
        }
        //if character running forward
        else if ((forward == 1 && actualForward <= 1) || (forward == -1 && actualForward >= -1))
        {
            PlaySoundClip(characterSoundEffectSet.rightFootstepSFX.audioClip, characterSoundEffectSet.rightFootstepSFX.runningVolume);
        }
        //if character walking forward
        else if ((forward == 0.5f && actualForward <= 0.5f) || (forward == -0.5f && actualForward >= -0.5f))
        {
            PlaySoundClip(characterSoundEffectSet.rightFootstepSFX.audioClip, characterSoundEffectSet.rightFootstepSFX.walkingVolume);
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
        //if character running left
        if ((left == 1 && actualLeft <= 1) || (left == -1 && actualLeft >= -1))
        {
            PlaySoundClip(characterSoundEffectSet.rightFootstepSFX.audioClip, characterSoundEffectSet.rightFootstepSFX.runningVolume);
        }
        //if character walking left
        else if ((left == 0.5f && actualLeft <= 0.5f) || (left == -0.5f && actualLeft >= -0.5f))
        {
            PlaySoundClip(characterSoundEffectSet.rightFootstepSFX.audioClip, characterSoundEffectSet.rightFootstepSFX.walkingVolume);
        }
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

        PlaySoundClip(characterSoundEffectSet.dodgeSFX.audioClip, characterSoundEffectSet.dodgeSFX.volume);
    }

    private void PlaySoundClip(AudioClip audioClip,float volume)
    {
        audioSourceHolder.locomotionSFX.PlayOneShot(audioClip);
        audioSourceHolder.locomotionSFX.volume = volume;
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
    #endregion
}
