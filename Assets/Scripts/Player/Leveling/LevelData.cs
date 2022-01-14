using UnityEngine;

[CreateAssetMenu(menuName = "PlayerData/XPAdvancementChart")]
public class LevelData : ScriptableObject
{
    public int maxLevel;
    public int maxXP;
    public AnimationCurve levelCurve;

}
