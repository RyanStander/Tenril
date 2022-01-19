using UnityEngine;

/// <summary>
/// Is meant to hold information and data relating to an enemy character
/// </summary>
public class EnemyStats : CharacterStats
{
    //[Header("Behaviour Traits")]
    //TODO: Create traits that affect the behaviour of the AI
    //public List<Traits> enemyTraits = new List<Traits>();

    public int xpToAwardOnDeath = 100;

    //The likeliness that an attack should be heavy
    [Range(0, 1)] public float heavyAttackLikeliness = 0.25f; 

    [Header("Awareness Abilities")]
    //"Blind" vision range, accounts for the area around the enemy
    public int alertRadius = 10;

    //Same as the alert radius, but an upper bound for when actively in combat
    public int maximumAlertRadius = 35;

    //Range for creature chasing, should ideally be greater than the alert and vision radius
    public float chaseRange = 15;

    //Same as the chase radius, but an upper bound for when actively in combat
    public float maximumChaseRange = 40;

    //Range at which they will inform other units to expand their alert radius
    public float informAlliesRadius = 10;

    //The chase speed of the AI
    [Range(0, 2)] public float chaseSpeed = 1;

    //Reposition speed of the AI
    [Range(0, 2)] public float repositionSpeed = 0.75f;

    //Range for creature knowing where obstacles that can be hidden behind are
    public float obstacleAwarenessRange = 15;

    //The range at which attacking should begin, should be replaced with preffered attacks
    public float maximumAttackRange = 3.5f;

    //The height difference allowed for attacking
    public float maximumAttackHeight = 0.5f;

    //The rotational speed for the AI
    [Range(1,10)] public float rotationSpeed = 5;

    //The rotational speed for the AI while attacking, recommended to be lower
    [Range(1, 10)] public float attackRotationSpeed = 2.5f;

    //Health threshold at which an enemy will want to attempt to heal
    public float healingThreshold = 0.25f;

    //Helper bool to track if healing is possible
    public bool canHeal = true;

    //Range at which the enemy will consider healing as an option
    public float healRange = 5;

    //Hiding health threshold at which an enemy will want to run away and hide
    public float hidingThreshold = 0.15f;

    //The time that an agent will wait between trying to hide again
    public float hidingCooldownTime = 30;

    //Helper bool to track if hiding is possible
    public bool canHide = true;

    private EnemyAgentManager enemyManager;

    [SerializeField] private SliderBarDisplayUI healthBar;
    
    private void Start()
    {
        SetupStats();

        //Get the animation manager
        enemyManager = GetComponent<EnemyAgentManager>();
        healthBar.SetMaxValue(maxHealth);
    }

    public override void TakeDamage(float damageAmount, bool playAnimation = true, string damageAnimation = "Hit")
    {
        //Return if already dead or invulnerable
        if (isDead || enemyManager.animatorManager.animator.GetBool("isInvulnerable")) return;

        //change current health
        base.TakeDamage(damageAmount);
        
        //update health display on the healthbar
        healthBar.SetCurrentValue(currentHealth);

        //Play hit animation if damage is taken
        if (playAnimation)
            enemyManager.animatorManager.PlayTargetAnimation(damageAnimation, true);

        //If character health reaches or goes past 0, play death animation and handle death
        if (currentHealth <= 0)
        {
            //Clamp the health to 0
            currentHealth = 0;

            //Play the death animation
            if (playAnimation) enemyManager.animatorManager.animator.Play("Death");

            //Set to dead in stats & in animator controller
            isDead = true;
            enemyManager.animatorManager.animator.SetBool("isDead", true);
        }
        
        //Highten their senses
        enemyManager.attackManager.HightenAlertChaseRadiuses();

        //Alert nearby enemies of attack and highten their senses
        enemyManager.attackManager.AlertAlliesOfAttack();
    }

    public override void RegainHealth(float regainAmount)
    {
        //Return if already dead
        if (isDead) return;

        //change current health
        base.RegainHealth(regainAmount);

        //update health display on the healthbar
        healthBar.SetCurrentValue(currentHealth);
    }
}
