using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.UI;

public class SliderValueText : MonoBehaviour
{
    public TextMeshProUGUI sliderText;
    public Slider slider;

    public void UpdateSliderText()
    {
        sliderText.text = slider.value.ToString();
    }
}
