using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public enum ProductionType
{
    MILITARY,
    INFRASTRUCTURE,
    SCIENCE,
    CULTURE
}

public class PlayerProduction : MonoBehaviour
{
    //Sliders that store a particular production type
    public int maximumProductionValue = 100;
    public Slider militarySlider;
    public Slider infrastructureSlider;
    public Slider scienceSlider;
    public Slider cultureSlider;

    private Dictionary<ProductionType, Slider> playerProductions = new Dictionary<ProductionType, Slider>();

    private void Start()
    {
        //Link the sliders to the production type dictionary
        playerProductions.Add(ProductionType.MILITARY, militarySlider);
        //playerProductions.Add(ProductionType.INFRASTRUCTURE, infrastructureSlider);
        //playerProductions.Add(ProductionType.SCIENCE, scienceSlider);
        //playerProductions.Add(ProductionType.CULTURE, cultureSlider);

        ////Set the maximum and minimum for the sliders
        //foreach(KeyValuePair<ProductionType,Slider> keyvalue in playerProductions)
        //{
        //    //Set the minimum and maximum
        //    keyvalue.Value.maxValue = 0;
        //    keyvalue.Value.maxValue = maximumProductionValue;
        //}
    }

    //For current stage testing
    public LeaderProductionTrait testTrait;
    public void Update()
    {
        testTrait.CalculateLikability(militarySlider.value, maximumProductionValue);
    }
}
