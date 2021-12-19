using UnityEngine;
/// <summary>
/// Manages leveling up
/// </summary>
public static class LevelSystem
{
    public static void DetermineLevelGain(LevelData levelData, int currentXP,int currentLevel,int addedXP)
    {
        int totalXP = currentXP + addedXP;

        float xpRequirement = GetXPForNextLevel(levelData, currentLevel);

        int levelsToGain = 0;
        //if player has enough to level up check how many levels are gained
        while (totalXP>=xpRequirement)
        {
            levelsToGain++;
            xpRequirement = GetXPForNextLevel(levelData, currentLevel, levelsToGain+1);
            Debug.Log("Level Up!");
        }

        if (levelsToGain>0)
        {
            EventManager.currentManager.AddEvent(new PlayerLevelUp(levelsToGain));
        }
    }

    public static float GetXPForNextLevel(LevelData levelData, int currentLevel,int levelIncrease=1)
    {
        float xpRequired = levelData.levelCurve.Evaluate((float)(currentLevel + levelIncrease)/levelData.maxLevel) * levelData.maxXP;
        //Get the xp required for the next level
        return xpRequired;
    }
}
