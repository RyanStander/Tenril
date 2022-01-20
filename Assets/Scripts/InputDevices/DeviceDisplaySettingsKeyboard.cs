using UnityEngine;

/// <summary>
/// Holds data for keyboard/mouse input device
/// </summary>
[CreateAssetMenu(menuName = "InputDevices/KeyboardMouseDisplaySettings")]
public class DeviceDisplaySettingsKeyboard : DeviceDisplaySettings
{
    [Header("Function keys")]
    public Sprite f1;
    public Sprite f2, f3, f4, f5, f6, f7, f8, f9, f10, f11, f12;

    [Header("Special keys")]
    public Sprite esc;
    public Sprite ctrl, leftAlt, rightAlt, tab, leftShit, rightShift, insert, delete, home, end, pageUp, pageDown, prtScreen, scrollLock, capsLock, numLock, backspace, enter, comma, dot, forwardSlash, 
        backSlash, quote, semiColon, minus, equal, leftSquareBracket, rightSquareBracket,spacebar,tilda;

    [Header("Number keys")]
    public Sprite number1; 
    public Sprite number2, number3, number4, number5, number6, number7, number8, number9, number0;

    [Header("Numpad keys")]
    public Sprite numSlash;
    public Sprite numAsterist, numMinus, num1, num2, num3, num4, num5, num6, num7, num8, num9, num0, numDot, numPlus, numEnter;

    [Header("Character keys")]
    public Sprite q;
    public Sprite w, e, r, t, y, u, i, o, p, a, s, d, f, g, h, j, k, l, z, x, c, v, b, n, m;

    [Header("Arrow keys")]
    public Sprite upArrow;
    public Sprite downArrow, leftArrow, rightArrow;

    [Header("Mouse")]
    public Sprite mouseLeft;
    public Sprite mouseMiddle,mouseRight,mouseBack,mouseForward;
}
