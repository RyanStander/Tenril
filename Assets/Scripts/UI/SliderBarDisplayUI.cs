using UnityEngine;
using UnityEngine.UI;

//Requires this component to function
[RequireComponent(typeof(Slider))]

public class SliderBarDisplayUI : MonoBehaviour
{
    //slider used to display the players current health
    private Slider barSlider;

    private void Awake()
    {
        barSlider = GetComponent<Slider>();
    }

    //called to set the max health that the player has
    public void SetMaxValue(float maxValue)
    {
        //checks if there is a fill rect set for the slider
        if (barSlider != null && barSlider.fillRect != null)
        {
            barSlider.maxValue = maxValue;
            barSlider.value = maxValue;
        }
        else
        {
            Debug.LogWarning("No slider fill for bar was found, player value cannot be updated. Please add a slider fill in the slider component");
        }
    }

    //updates the players current value
    public void SetCurrentValue(float currentValue)
    {
        //checks if there is a fill rect set for the slider
        if (barSlider != null && barSlider.fillRect != null)
        {
            barSlider.value = currentValue;
        }
        else
        {
            Debug.LogWarning("No slider fill for value bar was found, player value cannot be updated. Please add a slider fill in the slider component");
        }
    }
}
