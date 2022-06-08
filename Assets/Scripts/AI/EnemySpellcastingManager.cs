using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemySpellcastingManager : CharacterSpellcastingManager
{
    private EnemyAnimatorManager enemyAnimatorManager;
    private EnemyStats enemyStats;
    private EnemyAgentManager enemyAgentManager;

    private void Awake()
    {
        enemyAgentManager = GetComponent<EnemyAgentManager>();
        enemyStats = GetComponent<EnemyStats>();
        enemyAnimatorManager = GetComponent<EnemyAnimatorManager>();
    }

    public void SuccessfulyCastSpell()
    {
        if (enemyAgentManager == null)
            Debug.Log("could not find enemy manager");
        spellBeingCast.SuccessfullyCastSpell(enemyAnimatorManager, enemyStats, enemyAgentManager);
    }
}
