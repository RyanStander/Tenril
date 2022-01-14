using UnityEngine;
/// <summary>
/// Holds data for controller specific input devices
/// </summary>
[CreateAssetMenu(menuName = "InputDevices/ControllerDisplaySettings")]
public class DeviceDisplaySettingsController : DeviceDisplaySettings
{
    [Header("Display Name")] public string deviceDisplayName;

    [Header("Display Color")] public Color deviceDisplayColor;

    //Im not sure what this does
    [Header("Icon Settings")] public bool deviceHasCustomContext;

    [Header("Icons - Buttons")] public Sprite buttonNorthIcon;
    public Sprite buttonSouthIcon, buttonWestIcon, buttonEastIcon;

    [Header("Icons - Shoulder Buttons")] public Sprite leftTrigger;
    public Sprite leftShoulder, rightTrigger, rightShoulder;

    [Header("Icons - D-Pad")] public Sprite dPadLeft;
    public Sprite dPadRight, dPadUp, dPadDown;

    [Header("Icons - Analog Sticks")] public Sprite leftStickPress;
    public Sprite rightStickPress, leftStickUp, leftStickLeft, leftStickRight, leftStickDown, rightStickUp, rightStickLeft, rightStickRight, rightStickDown;

    [Header("Icons - Option Buttons")] public Sprite select;
    public Sprite start;

    [Header("Icons - Custom Context")]
    public CustomContext[] customContexts;
}
