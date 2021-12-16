using UnityEngine;

[CreateAssetMenu(menuName = "Sounds/WeaponSoundEffect")]
public class WeaponSoundEffects : ScriptableObject
{
    public AudioClipData weaponDrawSFX, weaponSwingSFX;
    public AudioClipData weaponHitFlesh, weaponHitStone, weaponHitWood, weaponHitMetal, weaponBlockAttack;
    //add more when can think of em
}

[System.Serializable]
public class AudioClipData
{
    [Tooltip("The audio clip to be played")]
    public AudioClip audioClip;
    [Tooltip("The volume level of the audio clip")]
    [Range(0, 1)] public float volume = 1f;
}
