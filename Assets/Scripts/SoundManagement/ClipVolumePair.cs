using UnityEngine;

/// <summary>
/// Simple struct to connect a clip and a volume value.
/// </summary>
public struct ClipVolumePair
{
    /// <summary>
    /// The audio clip to associate with a volume value.
    /// </summary>
    public readonly AudioClip audioClip;

    /// <summary>
    /// The audio volume to associate with an audio clip.
    /// </summary>
    public readonly float audioVolume;

    /// <summary>
    /// Constructor for quick creation of the struct.
    /// </summary>
    /// <param name="audioClip">The audio clip to associate with a volume value.</param>
    /// <param name="audioVolume">The audio volume to associate with an audio clip.</param>
    public ClipVolumePair(AudioClip audioClip, float audioVolume)
    {
        this.audioClip = audioClip;
        this.audioVolume = audioVolume;
    }
}
