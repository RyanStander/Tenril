using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "Enemy AI/Items/Healing")]
public class EnemyHealingItem : EnemyConsumable
{
    [Tooltip("The amount of health restored")]
    public int healAmount;

    public override void AttemptToUseItem(EnemyAgentManager enemyManager)
    {
        base.AttemptToUseItem(enemyManager);
    }

    public override void SuccessfullyUsedItem(EnemyAgentManager enemyManager)
    {
        base.SuccessfullyUsedItem(enemyManager);

        //Let player regain health
        enemyManager.enemyStats.RegainHealth(healAmount);
    }
}