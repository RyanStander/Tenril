using UnityEngine;

public class TimeDisplayRotator : MonoBehaviour
{
    private float timeStrength;
    [SerializeField]private RectTransform objectToRotate;
    //Determins if the rotation should increase or decrease
    [SerializeField] private bool approachingDaytime;
    private void OnEnable()
    {
        EventManager.currentManager.Subscribe(EventType.SendTimeStrength, OnReceiveTimeStrength);
    }

    private void OnDisable()
    {
        EventManager.currentManager.Unsubscribe(EventType.SendTimeStrength, OnReceiveTimeStrength);
    }

    private void Update()
    {
        float rotationValue;

        if (approachingDaytime)
        {
            rotationValue = 180+(180 * timeStrength);
            if (timeStrength >0.99999f)
            {
                approachingDaytime = false;
            }

        }
        else
        {
            rotationValue = 180 - (180 * timeStrength);
            if (timeStrength <0.00001f)
            {
                approachingDaytime = true;
            }
        }
        Debug.Log(rotationValue);
        objectToRotate.rotation=Quaternion.Euler(0,0, rotationValue);

        //0 str=180 rotation
        //1 str=360 rotation
        //0-1 = 180-360
        //1-0= 0-180
    }

    private void OnReceiveTimeStrength(EventData eventData)
    {
        if (eventData is SendTimeStrength sendTimeStrength)
        {
            timeStrength = sendTimeStrength.timeStrength;
        }
        else
        {
            throw new System.Exception("Error: EventData class with EventType.SendTimeStrength was received but is not of class SendTimeStrength.");
        }
    }
}
