using System;
using UnityEngine;

/// <summary>
/// Holder object for the different texture sound footstep types for characters.
/// </summary>
[CreateAssetMenu(menuName = "Sounds/TextureBased/TextureFootstepSFX")]
[Serializable]
public class TextureFootstepSFX : ScriptableObject
{
    /// <summary>
    /// The sounds that will be used when a character walks.
    /// </summary>
    [field: SerializeField]
    public TextureSounds[] WalkingSounds { get; private set; }

    /// <summary>
    /// The sounds that will be used when a character runs.
    /// </summary>
    [field: SerializeField]
    public TextureSounds[] RunningSounds { get; private set; }

    /// <summary>
    /// The sounds that will be used when a character sprints.
    /// </summary>
    [field: SerializeField]
    public TextureSounds[] SprintingSounds { get; private set; }

    /// <summary>
    /// The sounds that will be used when no common texture is found.
    /// </summary>
    [field: SerializeField]
    public TextureSounds UndefinedFootstepsSounds { get; private set; }
}