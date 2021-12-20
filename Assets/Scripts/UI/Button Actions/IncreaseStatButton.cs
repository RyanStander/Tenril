using UnityEngine;

/// <summary>
/// Increases a player stat by sending a value
/// </summary>
public class IncreaseStatButton : MonoBehaviour
{
    [SerializeField] private Skill skillToIncrease;
    [SerializeField] private bool consumeSkillPoint;
    public void LevelStat()
    {
        EventManager.currentManager.AddEvent(new PlayerGainSkill(skillToIncrease, consumeSkillPoint));
    }
}
