using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RaycastPathing : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        Debug.Log("Hello World!");
        TestRandomNumberPrint();
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    //Test function for GitKraken script merging
    private void TestRandomNumberPrint()
    {
        Debug.Log("My random number is " + Random.Range(1,11) + "!");
    }
}
