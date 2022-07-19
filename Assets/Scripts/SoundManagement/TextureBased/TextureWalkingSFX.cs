using System;
using UnityEngine;

/// <summary>
/// Data class for walking sounds assosiated with given textures.
/// </summary>
[CreateAssetMenu(menuName = "Sounds/TextureWalkingSFX")]
[Serializable]
public class TextureWalkingSFX : TextureSound
{
    /// <summary>
    /// Volume levels for the different movement states.
    /// </summary>
    [Tooltip("The volume level based on how fast the character is running")]
    [Range(0, 1)] public float walkingVolume = 0.5f, runningVolume = 0.75f, sprintingVolume = 1f;
}
