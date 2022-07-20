using System;
using System.Linq;
using UnityEngine;

/// <summary>
/// Data class for sounds assosiated with given textures.
/// </summary>
[CreateAssetMenu(menuName = "Sounds/TextureSound")]
[Serializable]
public class TextureSounds : ScriptableObject
{
    /// <summary>
    /// The texture of the class.
    /// </summary>
    public Texture[] albedos;

    /// <summary>
    /// The sound clips assosiated with the class.
    /// </summary>
    public AudioClip[] audioClips;

    /// <summary>
    /// The clip that was last played.
    /// </summary>
    private AudioClip previouslyPlayedClip;

    /// <summary>
    /// The volume level of the audio collection.
    /// </summary>
    [Range(0, 1)]
    public float audioVolume = 1;

    /// <summary>
    /// Gets a random audio clip from the texture sound.
    /// </summary>
    /// <returns>A random audio clip.</returns>
    public AudioClip GetRandomClip() => audioClips[UnityEngine.Random.Range(0, audioClips.Length)];

    /// <summary>
    /// Gets a random audio clip from the texture sound.
    /// This method avoids getting the same clip twice in a row.
    /// </summary>
    /// <returns>A near-random audio clip.</returns>
    public AudioClip GetUniqueRandomClip()
    {
        // If only one clip exists, skip a unique fetch.
        if(audioClips.Length == 1)
        {
            return GetRandomClip();
        }
        
        // Filter out the previously played clip and get a random sound.
        AudioClip[] filteredClips = audioClips.Where(x => x != previouslyPlayedClip).ToArray();
        previouslyPlayedClip = filteredClips[UnityEngine.Random.Range(0, filteredClips.Length)];
        return previouslyPlayedClip;
    }

    /// <summary>
    /// Gets a random audio clip pair from the texture sound.
    /// This method avoids getting the same clip twice in a row.
    /// </summary>
    /// <returns>A near-random audio clip paired with volume.</returns>
    public ClipVolumePair GetUniqueRandomClipPair() => new ClipVolumePair(GetUniqueRandomClip(), audioVolume);
}
