using System;
using UnityEngine;

/// <summary>
/// Holder object for the jumping and landing texture sounds for characters.
/// </summary>
[CreateAssetMenu(menuName = "Sounds/TextureBased/TextureJumpCycleSFX")]
[Serializable]
public class TextureJumpCycleSFX : ScriptableObject
{
    /// <summary>
    /// The sounds that will be used when a character jumps.
    /// </summary>
    [field: SerializeField]
    public TextureSounds[] JumpingSounds { get; private set; }

    /// <summary>
    /// The sounds that will be used when a character lands.
    /// </summary>
    [field: SerializeField]
    public TextureSounds[] LandingSounds { get; private set; }

    /// <summary>
    /// The sounds that will be used when no common texture is found.
    /// </summary>
    [field: SerializeField]
    public TextureSounds UndefinedJumpingSounds { get; private set; }

    /// <summary>
    /// The sounds that will be used when no common texture is found.
    /// </summary>
    [field: SerializeField]
    public TextureSounds UndefinedLandingSounds { get; private set; }
}