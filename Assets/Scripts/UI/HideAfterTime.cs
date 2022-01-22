using UnityEngine;

public class HideAfterTime : MonoBehaviour
{
    [SerializeField]private float waitTimeInSeconds = 1;
    private float timeStamp;
    private bool isActive;

    private void FixedUpdate()
    {
        if (isActive)
        {
            if (timeStamp<Time.time)
            {
                isActive = false;
                gameObject.SetActive(false);
            }
        }
        else if(gameObject.activeSelf)
        {
            isActive = true;
            timeStamp = waitTimeInSeconds + Time.time;
        }
    }
}
