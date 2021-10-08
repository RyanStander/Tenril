using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class StatusEffectIcon : MonoBehaviour
{
    public Image image;
    public TMP_Text timeLeftDisplay;
    private float timeLeft;

    private void Update()
    {
        timeLeftDisplay.text = (int)(timeLeft - Time.time)+"s";
    }

    internal void SetValues(Sprite sprite, float timeLeft)
    {
        this.timeLeft = timeLeft;
        image.sprite = sprite;
    }
}
