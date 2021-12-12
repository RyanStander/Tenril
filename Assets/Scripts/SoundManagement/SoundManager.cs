using UnityEngine;

[RequireComponent(typeof(AudioSource))]
public class SoundManager : MonoBehaviour
{
    private AudioSource audioSource;
    private Animator animator;
    [SerializeField] private CharacterSoundEffects characterSoundEffectSet;


    private void Start()
    {
        audioSource = GetComponent<AudioSource>();
        animator = GetComponent<Animator>();
    }

    private void ForwardFootstepL(float forward)
    {
        if (characterSoundEffectSet == null)
        {
            Debug.LogWarning("no sound effect set for the character, make sure you have set it");
            return;
        }

        float actualForward = animator.GetFloat("Forward");

        //if character sprinting forward
        if (forward==-2f || forward== 2f)
        {
            PlaySelectedSoundClip(characterSoundEffectSet.leftFootstepSFX.audioClip, characterSoundEffectSet.leftFootstepSFX.sprintingVolume);
        }
        //if character running forward
        else if ((forward==1f&&actualForward<= 1f) ||(forward==-1f && actualForward >= -1f))
        {
            PlaySelectedSoundClip(characterSoundEffectSet.leftFootstepSFX.audioClip, characterSoundEffectSet.leftFootstepSFX.runningVolume);
        }
        //if character walking forward
        else if ((forward == 0.5f &&actualForward<=0.5f) || (forward == -0.5f && actualForward >= -0.5f))
        {
            PlaySelectedSoundClip(characterSoundEffectSet.leftFootstepSFX.audioClip, characterSoundEffectSet.leftFootstepSFX.walkingVolume);
        }
    }

    private void LeftFootstepL(float left)
    {
        Debug.Log("Performing left step");
        if (characterSoundEffectSet == null)
        {
            Debug.LogWarning("no sound effect set for the character, make sure you have set it");
            return;
        }

        float actualLeft = animator.GetFloat("Left");
        float actualForward = animator.GetFloat("Forward");

        if (!(actualForward <= 0.0001f && actualForward >= -0.0001f))
        {
            return;
        }
        Debug.Log("step performed");
        //if character running left
        if ((left == 1 && actualLeft <= 1)||(left==-1&&actualLeft>=-1))
        {
            PlaySelectedSoundClip(characterSoundEffectSet.leftFootstepSFX.audioClip, characterSoundEffectSet.leftFootstepSFX.runningVolume);
        }
        //if character walking left
        else if ((left == 0.5f && actualLeft <= 0.5f)|| (left == -0.5f && actualLeft >= -0.5f))
        {
            PlaySelectedSoundClip(characterSoundEffectSet.leftFootstepSFX.audioClip, characterSoundEffectSet.leftFootstepSFX.walkingVolume);
        }
    }

    private void ForwardFootstepR(float forward)
    {
        if (characterSoundEffectSet == null)
        {
            Debug.LogWarning("no sound effect set for the character, make sure you have set it");
            return;
        }

        float actualForward = animator.GetFloat("Forward");

        //if character sprinting forward
        if (forward == -2 || forward == 2)
        {
            PlaySelectedSoundClip(characterSoundEffectSet.rightFootstepSFX.audioClip, characterSoundEffectSet.rightFootstepSFX.sprintingVolume);
        }
        //if character running forward
        else if ((forward == 1 && actualForward <= 1) || (forward == -1 && actualForward >= -1))
        {
            PlaySelectedSoundClip(characterSoundEffectSet.rightFootstepSFX.audioClip, characterSoundEffectSet.rightFootstepSFX.runningVolume);
        }
        //if character walking forward
        else if ((forward == 0.5f && actualForward <= 0.5f) || (forward == -0.5f && actualForward >= -0.5f))
        {
            PlaySelectedSoundClip(characterSoundEffectSet.rightFootstepSFX.audioClip, characterSoundEffectSet.rightFootstepSFX.walkingVolume);
        }
    }

    private void LeftFootstepR(float left)
    {
        Debug.Log("Performing left step");
        if (characterSoundEffectSet == null)
        {
            Debug.LogWarning("no sound effect set for the character, make sure you have set it");
            return;
        }

        float actualLeft = animator.GetFloat("Left");
        float actualForward = animator.GetFloat("Forward");

        if (!(actualForward <= 0.0001f && actualForward >= -0.0001f))
        {
            return;
        }
        Debug.Log("step performed");
        //if character running left
        if ((left == 1 && actualLeft <= 1) || (left == -1 && actualLeft >= -1))
        {
            PlaySelectedSoundClip(characterSoundEffectSet.rightFootstepSFX.audioClip, characterSoundEffectSet.rightFootstepSFX.runningVolume);
        }
        //if character walking left
        else if ((left == 1 && actualLeft <= 1) || (left == -1 && actualLeft >= -1))
        {
            PlaySelectedSoundClip(characterSoundEffectSet.rightFootstepSFX.audioClip, characterSoundEffectSet.rightFootstepSFX.walkingVolume);
        }
    }

    private void JumpStart(float volume = 1f)
    {
        audioSource.PlayOneShot((AudioClip)Resources.Load(""));
        audioSource.volume = volume;
    }

    private void JumpLand(float volume = 1f)
    {
        audioSource.PlayOneShot((AudioClip)Resources.Load(""));
        audioSource.volume = volume;
    }

    private void PlaySelectedSoundClip(AudioClip audioClip,float volume)
    {
        audioSource.PlayOneShot(audioClip);
        //audioSource.volume = volume;
    }
}

//private void FootstepR(float forward, float left, float volume = 1f)
//{
//    float actualForward = animator.GetFloat("Forward");
//    float actualLeft = animator.GetFloat("Left");

//    //if character sprinting forward
//    if (forward > 1)
//    {
//        PlaySelectedSoundClip(characterSoundEffectSet.rightFootstepSFX.audioClip, volume);
//    }
//    //if character running forward
//    else if (forward > 0.5f && actualForward <= 1)
//    {
//        PlaySelectedSoundClip(characterSoundEffectSet.rightFootstepSFX.audioClip, volume);
//    }
//    //if character walking forward
//    else if (forward > 0 && actualForward <= 0.5f)
//    {
//        PlaySelectedSoundClip(characterSoundEffectSet.rightFootstepSFX.audioClip, volume);
//    }
//    //if character running left
//    else if (left > 0.5f && actualLeft <= 1)
//    {
//        PlaySelectedSoundClip(characterSoundEffectSet.rightFootstepSFX.audioClip, volume);
//    }
//    else if (left > 0 && actualLeft <= 0.5f)
//    {
//        PlaySelectedSoundClip(characterSoundEffectSet.rightFootstepSFX.audioClip, volume);
//    }
//}
