
using UnityEngine;
using UnityEngine.InputSystem;
/// <summary>
/// The CUM serves as a helper class that holds methods for utility. Let the CUM enter your scripts.
/// </summary>
public static class CharacterUtilityManager
{
    /// <summary>
    /// Returns the amount of damage that a character would take depending on its resistances and the attackers damage
    /// whilst blocking.
    /// </summary>
    public static float CalculateBlockingDamage(float currentWeaponDamage, float blockingPhysicalDamageAbsorption)
    {
        return currentWeaponDamage - (currentWeaponDamage * blockingPhysicalDamageAbsorption) / 100;
        //TODO: This currently only uses weapon damage and blocking damage absorption, for future needs resistances and damage bonuses taken into account
    }


    public static bool CheckIfHitColliderOnLayer(Vector3 attackerPosition, Vector3 defenderPosition, LayerMask layerToCheck)
    {
        //calculate the direction and distance between the attacker and defender
        Vector3 direction = defenderPosition - attackerPosition;
        float distance =Vector3.Distance(attackerPosition, defenderPosition);
        RaycastHit hit;

        if (Physics.Raycast(attackerPosition, direction, out hit, distance, layerToCheck))
        {
            if (hit.transform != null)
                return true;
        }
        return false;
    }

    #region Player Specific
    public static Sprite FindKeybindIcon(DeviceDisplayConfigurator deviceDisplayConfigurator,InputDeviceType activeInputType,string bindingPath)
    {
        switch (activeInputType)
        {
            case InputDeviceType.KeyboardMouse:
                return FindKeybindIconForKeyboardAndMouse(deviceDisplayConfigurator.keyboardAndMouseSettings.deviceDisplaySettings, bindingPath);
            case InputDeviceType.GeneralGamepad:
                return FindKeybindIconForController(deviceDisplayConfigurator.xboxSettings.deviceDisplaySettings, bindingPath);
            case InputDeviceType.PlayStation:
                return FindKeybindIconForController(deviceDisplayConfigurator.playstationSettings.deviceDisplaySettings, bindingPath);
            case InputDeviceType.Xbox:
                return FindKeybindIconForController(deviceDisplayConfigurator.xboxSettings.deviceDisplaySettings, bindingPath);
        }
        return null;
    }

    private static Sprite FindKeybindIconForKeyboardAndMouse(DeviceDisplaySettings deviceDisplaySettings, string bindingPath)
    {
        if (deviceDisplaySettings is DeviceDisplaySettingsKeyboard keyboardIcons)
        {
            switch (bindingPath)
            {
                case "<Keyboard>/f1":
                    return keyboardIcons.f1;
                case "<Keyboard>/f2":
                    return keyboardIcons.f2;
                case "<Keyboard>/f3":
                    return keyboardIcons.f3;
                case "<Keyboard>/f4":
                    return keyboardIcons.f4;
                case "<Keyboard>/f5":
                    return keyboardIcons.f5;
                case "<Keyboard>/f6":
                    return keyboardIcons.f6;
                case "<Keyboard>/f7":
                    return keyboardIcons.f7;
                case "<Keyboard>/f8":
                    return keyboardIcons.f8;
                case "<Keyboard>/f9":
                    return keyboardIcons.f9;
                case "<Keyboard>/f10":
                    return keyboardIcons.f10;
                case "<Keyboard>/f11":
                    return keyboardIcons.f11;
                case "<Keyboard>/f12":
                    return keyboardIcons.f12;
                case "<Keyboard>/escape":
                    return keyboardIcons.esc;
                case "<Keyboard>/ctrl":
                    return keyboardIcons.ctrl;
                case "<Keyboard>/leftAlt":
                    return keyboardIcons.leftAlt;
                case "<Keyboard>/rightAlt":
                    return keyboardIcons.rightAlt;
                case "<Keyboard>/alt":
                    return keyboardIcons.leftAlt;
                case "<Keyboard>/tab":
                    return keyboardIcons.tab;
                case "<Keyboard>/leftShift":
                    return keyboardIcons.leftShit;
                case "<Keyboard>/rightShift":
                    return keyboardIcons.rightShift;
                case "<Keyboard>/shift":
                    return keyboardIcons.leftShit;
                case "<Keyboard>/insert":
                    return keyboardIcons.insert;
                case "<Keyboard>/delete":
                    return keyboardIcons.delete;
                case "<Keyboard>/home":
                    return keyboardIcons.home;
                case "<Keyboard>/end":
                    return keyboardIcons.end;
                case "<Keyboard>/pageUp":
                    return keyboardIcons.pageUp;
                case "<Keyboard>/pageDown":
                    return keyboardIcons.pageDown;
                case "<Keyboard>/printScreen":
                    return keyboardIcons.prtScreen;
                case "<Keyboard>/scrollLock":
                    return keyboardIcons.scrollLock;
                case "<Keyboard>/capsLock":
                    return keyboardIcons.capsLock;
                case "<Keyboard>/numLock":
                    return keyboardIcons.numLock;
                case "<Keyboard>/backspace":
                    return keyboardIcons.backspace;
                case "<Keyboard>/enter":
                    return keyboardIcons.enter;
                case "<Keyboard>/comma":
                    return keyboardIcons.comma;
                case "<Keyboard>/period":
                    return keyboardIcons.dot;
                case "<Keyboard>/slash":
                    return keyboardIcons.forwardSlash;
                case "<Keyboard>/backslash":
                    return keyboardIcons.backSlash;
                case "<Keyboard>/quote":
                    return keyboardIcons.quote;
                case "<Keyboard>/semicolon":
                    return keyboardIcons.semiColon;
                case "<Keyboard>/minus":
                    return keyboardIcons.minus;
                case "<Keyboard>/equals":
                    return keyboardIcons.equal;
                case "<Keyboard>/leftBracket":
                    return keyboardIcons.leftSquareBracket;
                case "<Keyboard>/rightBracket":
                    return keyboardIcons.rightSquareBracket;
                case "<Keyboard>/backquote":
                    return keyboardIcons.tilda;
                case "<Keyboard>/1":
                    return keyboardIcons.number1;
                case "<Keyboard>/2":
                    return keyboardIcons.number2;
                case "<Keyboard>/3":
                    return keyboardIcons.number3;
                case "<Keyboard>/4":
                    return keyboardIcons.number4;
                case "<Keyboard>/5":
                    return keyboardIcons.number5;
                case "<Keyboard>/6":
                    return keyboardIcons.number6;
                case "<Keyboard>/7":
                    return keyboardIcons.number7;
                case "<Keyboard>/8":
                    return keyboardIcons.number8;
                case "<Keyboard>/9":
                    return keyboardIcons.number9;
                case "<Keyboard>/0":
                    return keyboardIcons.number0;
                case "<Keyboard>/numpadDivide":
                    return keyboardIcons.numSlash;
                case "<Keyboard>/numpadMultiply":
                    return keyboardIcons.numAsterist;
                case "<Keyboard>/numpadMinus":
                    return keyboardIcons.numMinus;
                case "<Keyboard>/numpad1":
                    return keyboardIcons.num1;
                case "<Keyboard>/numpad2":
                    return keyboardIcons.num2;
                case "<Keyboard>/numpad3":
                    return keyboardIcons.num3;
                case "<Keyboard>/numpad4":
                    return keyboardIcons.num4;
                case "<Keyboard>/numpad5":
                    return keyboardIcons.num5;
                case "<Keyboard>/numpad6":
                    return keyboardIcons.num6;
                case "<Keyboard>/numpad7":
                    return keyboardIcons.num7;
                case "<Keyboard>/numpad8":
                    return keyboardIcons.num8;
                case "<Keyboard>/numpad9":
                    return keyboardIcons.num9;
                case "<Keyboard>/numpad0":
                    return keyboardIcons.num0;
                case "<Keyboard>/numpadPeriod":
                    return keyboardIcons.numDot;
                case "<Keyboard>/numpadPlus":
                    return keyboardIcons.numPlus;
                case "<Keyboard>/numpadEnter":
                    return keyboardIcons.numEnter;
                case "<Keyboard>/q":
                    return keyboardIcons.q;
                case "<Keyboard>/w":
                    return keyboardIcons.w;
                case "<Keyboard>/e":
                    return keyboardIcons.e;
                case "<Keyboard>/r":
                    return keyboardIcons.r;
                case "<Keyboard>/t":
                    return keyboardIcons.t;
                case "<Keyboard>/y":
                    return keyboardIcons.y;
                case "<Keyboard>/u":
                    return keyboardIcons.u;
                case "<Keyboard>/i":
                    return keyboardIcons.i;
                case "<Keyboard>/o":
                    return keyboardIcons.o;
                case "<Keyboard>/p":
                    return keyboardIcons.p;
                case "<Keyboard>/a":
                    return keyboardIcons.a;
                case "<Keyboard>/s":
                    return keyboardIcons.s;
                case "<Keyboard>/d":
                    return keyboardIcons.d;
                case "<Keyboard>/f":
                    return keyboardIcons.f;
                case "<Keyboard>/g":
                    return keyboardIcons.g;
                case "<Keyboard>/h":
                    return keyboardIcons.h;
                case "<Keyboard>/j":
                    return keyboardIcons.j;
                case "<Keyboard>/k":
                    return keyboardIcons.k;
                case "<Keyboard>/l":
                    return keyboardIcons.l;
                case "<Keyboard>/z":
                    return keyboardIcons.z;
                case "<Keyboard>/x":
                    return keyboardIcons.x;
                case "<Keyboard>/c":
                    return keyboardIcons.c;
                case "<Keyboard>/v":
                    return keyboardIcons.v;
                case "<Keyboard>/b":
                    return keyboardIcons.b;
                case "<Keyboard>/n":
                    return keyboardIcons.n;
                case "<Keyboard>/m":
                    return keyboardIcons.m;
                case "<Keyboard>/upArrow":
                    return keyboardIcons.upArrow;
                case "<Keyboard>/downArrow":
                    return keyboardIcons.downArrow;
                case "<Keyboard>/leftArrow":
                    return keyboardIcons.leftArrow;
                case "<Keyboard>/rightArrow":
                    return keyboardIcons.rightArrow;
                case "<Mouse>/leftButton":
                    return keyboardIcons.mouseLeft;
                case "<Mouse>/middleButton":
                    return keyboardIcons.mouseMiddle;
                case "<Mouse>/rightButton":
                    return keyboardIcons.mouseRight;
                default:
                    Debug.LogWarning("Could not find binding: " + bindingPath);
                    return null;
                    //keep this in case binding is required
                    //case "<Keyboard>/":
                    //return keyboardIcons.;
            }
        }
        else
            return null;
    }

    private static Sprite FindKeybindIconForController(DeviceDisplaySettings deviceDisplaySettings, string bindingPath)
    {
        if (deviceDisplaySettings is DeviceDisplaySettingsController controllerIcons)
        {
            switch (bindingPath)
            {
                #region Buttons
                case "<Gamepad>/buttonNorth":
                    return controllerIcons.buttonNorthIcon;

                case "<Gamepad>/buttonSouth":
                    return controllerIcons.buttonSouthIcon;

                case "<Gamepad>/buttonWest":
                    return controllerIcons.buttonWestIcon;

                case "<Gamepad>/buttonEast":
                    return controllerIcons.buttonEastIcon;

                #endregion
                #region Shoulder Buttons
                case "<Gamepad>/leftShoulder":
                    return controllerIcons.leftShoulder;

                case "<Gamepad>/leftTrigger":
                    return controllerIcons.leftTrigger;

                case "<Gamepad>/rightShoulder":
                    return controllerIcons.rightShoulder;

                case "<Gamepad>/rightTrigger":
                    return controllerIcons.rightShoulder;

                #endregion
                #region d-Pad
                case "<Gamepad>/dpad/down":
                    return controllerIcons.dPadDown;

                case "<Gamepad>/dpad/up":
                    return controllerIcons.dPadUp;

                case "<Gamepad>/dpad/left":
                    return controllerIcons.dPadLeft;

                case "<Gamepad>/dpad/right":
                    return controllerIcons.dPadRight;
                #endregion
                #region Analog Sticks
                //This is where i put my analog stick keybinds IF I HAD ANY
                #endregion
                #region Option Buttons
                case "<Gamepad>/select":
                    return controllerIcons.select;

                case "<Gamepad>/start":
                    return controllerIcons.start;

                #endregion
                default:
                    Debug.LogWarning("Couldnt find matching gamepade selection");
                    return null;
            }
        }
        else
        {
            Debug.LogWarning("Couldnt find matching gamepade selection");
            return null;
        }
    }

    public static string GetBindingPath(InputAction inputAction, InputDeviceType inputDeviceType)
    {
        switch (inputDeviceType)
        {
            case InputDeviceType.KeyboardMouse:
                return inputAction.bindings[0].effectivePath;
            case InputDeviceType.GeneralGamepad:
                return inputAction.bindings[1].effectivePath;
            case InputDeviceType.PlayStation:
                return inputAction.bindings[1].effectivePath;
            case InputDeviceType.Xbox:
                return inputAction.bindings[1].effectivePath;
        }
        return null;
    }
    #endregion
}

[System.Serializable]
public class ItemInventory
{
    /// <summary>
    /// The item in the inventory slot
    /// </summary>
    public Item item;
    /// <summary>
    /// How many of the item is in the stack
    /// </summary>
    public int itemStackCount;
}
