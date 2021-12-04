using UnityEngine;

//Script is meant to help with testing and debugging events or actions related to changes in health
public class AutomaticDamageApplier : MonoBehaviour
{
    //The character being affected
    public CharacterStats characterStats;

    //The health that should be reduced
    public int healthToReduce;

    //Bool for if the damage should apply over time or once
    public bool isOverTimeDamager;

    //Bool for if this should happen on fixed or normal update
    public bool isFixedUpdate;

    // Start is called before the first frame update
    void Start()
    {
        //If character stats is null
        if(characterStats == null)
        {
            //Get the component for it
            characterStats = GetComponent<CharacterStats>();
        }

        //Reduce health
        ApplyDamage();
    }

    private void Update()
    {
        //If using update, reduce health
        if (!isFixedUpdate && isOverTimeDamager) ApplyDamage();
    }

    private void FixedUpdate()
    {
        //If using fixed update, reduce health
        if (isFixedUpdate && isOverTimeDamager) ApplyDamage();
    }

    private void ApplyDamage()
    {
        //Apply damage based on if it is one time or over time
        if(isOverTimeDamager)
        {
            //Cast down to character stats
            characterStats.TakeDamage(healthToReduce * Time.deltaTime, false, "Hit");
        }
        else
        {
            //Cast down to character stats
            characterStats.TakeDamage(healthToReduce, false, "Hit");
        }


    }
}
