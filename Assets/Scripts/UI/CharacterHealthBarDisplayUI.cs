using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CharacterHealthBarDisplayUI : SliderBarDisplayUI
{
    // Update is called once per frame
    void Update()
    {
        //Disable the bar if the value hits 0
        if(barSlider.value <= 0)
        {
            barSlider.gameObject.SetActive(false);
        }
    }
}
