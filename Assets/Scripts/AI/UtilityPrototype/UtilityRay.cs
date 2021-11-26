using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UtilityRay
{
    //Constructor
    public UtilityRay(Ray givenRay, float givenUtility, Color givenColor)
    {
        baseRay = givenRay;
        rayUtility = givenUtility;
        rayColor = givenColor;
    }

    public UtilityRay() { }

    //Ray that contains ray utilityRay information
    public Ray baseRay;

    //The utility value (favorability) of the ray
    public float rayUtility;

    //The color that the ray (if debbuging) should have
    public Color rayColor;
}
