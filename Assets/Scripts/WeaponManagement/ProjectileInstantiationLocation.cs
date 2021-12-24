using UnityEngine;
/// <summary>
/// Attach to a location you wish to instantiate a projectile on firing. Alternatively setting an override will allow you to place a seperate object for instantiation
/// </summary>

public class ProjectileInstantiationLocation : MonoBehaviour
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
