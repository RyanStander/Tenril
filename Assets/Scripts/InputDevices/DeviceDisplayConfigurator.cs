using UnityEngine;

[CreateAssetMenu(menuName = "InputDevices/DeviceDisplayConfigurator")]
public class DeviceDisplayConfigurator : ScriptableObject
{
    public DeviceSet[] deviceSets;

    [System.Serializable]
    public class DeviceSet
    {
        /// <summary>
        /// the path of the device
        /// </summary>
        public string rawPathName;
        /// <summary>
        /// The deviceDisplaySettings for the device that will be shown
        /// </summary>
        public DeviceDisplaySettings deviceDisplaySettings;
    }
}
