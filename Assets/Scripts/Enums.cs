public enum AmmoType
{
    Arrow=0,
    Bolt=1,
}

public enum WeaponType
{
    FlyingMage=0,
    Polearm=1,
    TwoHandedSword=2,
    Bow=3,
    DualBlades=4,
}

public enum Material
{
    Flesh=0,
    Wood=1,
    Stone=2,
    Metal=3,
}

public enum Skill
{
    Health=0,
    Stamina=1,
    Moonlight=2,
    Sunlight=3,
}

public enum SpellType
{
    none=0,
    biomancy=1,
    pyromancy=2,
}

//Perhaps we can implement more but this will do for now
public enum InputDeviceType
{
    KeyboardMouse=0,
    GeneralGamepad=1,
    PlayStation=2,
    Xbox=3,
}

//When we find more tools, will add here
public enum ToolType
{
    None=0,
    Pickaxe=1,
    Axe=2,
}

public enum PlayerState
{
    Default=0,
    CombatMode=1,
    SpellcastingMode=2,
    MenuMode=3,
    Dead=10,
}