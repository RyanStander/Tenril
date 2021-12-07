using UnityEngine;

/// <summary>
/// holds the data for the spawning of live arrows, which is to say, the arrow that you will see when released and fired in the aimed direction
/// </summary>
public class ArrowInstantiationLocation : MonoBehaviour
{
    [Tooltip("If true it will use the override transform, if false it will use the transform the the script is attached to")]
    public bool hasOverride;
    [Tooltip("If you wish to override the location of the instantiation, set the override transform here")]
    public Transform transformOverrideLocation;

    public Transform GetTransform()
    {
        if (hasOverride)
        {
            if (transformOverrideLocation == null)
            {
                Debug.LogWarning("Arrow instantiation location for bow's override location was set to true but a transform for the override was not set. please do so");
                return transform;
            }
            else
                return transformOverrideLocation;
        }
        else
            return transform;
    }
}
