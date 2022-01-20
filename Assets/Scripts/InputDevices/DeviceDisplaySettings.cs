using UnityEngine;
/// <summary>
/// Base class for device display settings, used for keeping data on controller inputs
/// </summary>
public class DeviceDisplaySettings : ScriptableObject
{
    [Header("Display Name")] public InputDeviceType deviceDisplayName;

    [Header("Display Color")] public Color deviceDisplayColor;

    [System.Serializable]
    public class CustomContext
    {
        /// <summary>
        /// The name of the input binding
        /// </summary>
        public string inputBindingString;
        /// <summary>
        /// The icon of the custom context
        /// </summary>
        public Sprite displayIcon;
    }
}


