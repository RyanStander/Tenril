using System;
using UnityEngine;

/// <summary>
/// Data class for sounds assosiated with a given texture.
/// </summary>
[CreateAssetMenu(menuName = "Sounds/TextureSound")]
[Serializable]
public class TextureSound : ScriptableObject
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
    /// Gets a random audio clip from the texture sound.
    /// </summary>
    /// <returns>A random audio clip.</returns>
    public AudioClip GetRandomClip() => audioClips[UnityEngine.Random.Range(0, audioClips.Length)];
}
