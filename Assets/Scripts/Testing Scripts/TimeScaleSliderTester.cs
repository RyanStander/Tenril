using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class TimeScaleSliderTester : MonoBehaviour
{
    //Slider being tracked
    public Slider timeSlider;

    private void Start()
    {
        //Start at the correct time scale
        timeSlider.value = Time.timeScale;
    }

    //Update the time scale based on the slider value
    public void UpdateTimeScale()
    {
        if(timeSlider != null) Time.timeScale = timeSlider.value;
    }
}
