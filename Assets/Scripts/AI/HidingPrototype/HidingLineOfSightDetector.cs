using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HidingLineOfSightDetector : MonoBehaviour
{
    //Collider of the object
    public SphereCollider visionCollider;

    //Field of vision that affects at what angle something can be seen
    public float fieldOfView = 90f;

    //Layers that should affect line of sight
    public LayerMask lineOfSightLayers;

    //Events that will be used when determining if a target is seen
    public delegate void GainSightEvent(Transform target);
    public GainSightEvent OnGainSight;
    public delegate void LoseSightEvent(Transform target);
    public LoseSightEvent OnLoseSight;

    //Courotine that will be used to check for sight
    private Coroutine CheckForLineOfSightCoroutine;

    private void Awake()
    {
        //Get the component automatically
        visionCollider = GetComponent<SphereCollider>();
    }

    private void OnTriggerEnter(Collider other)
    {
        //If line of sight is valid
        if (!CheckLineOfSight(other.transform))
        {
            //Set and start the courotine
            CheckForLineOfSightCoroutine = StartCoroutine(CheckForLineOfSight(other.transform));
        }
    }

    private void OnTriggerExit(Collider other)
    {
        //Check if sight was lost
        OnLoseSight?.Invoke(other.transform);

        //If not null, then stop the sight check
        if (CheckForLineOfSightCoroutine != null)
        {
            //Stop the courotine
            StopCoroutine(CheckForLineOfSightCoroutine);
        }
    }

    private bool CheckLineOfSight(Transform Target)
    {
        //Direction between the hider and target to run from
        Vector3 direction = (Target.transform.position - transform.position).normalized;

        //Number between -1 and 1, where in relation to the forward are we currently
        float dotProduct = Vector3.Dot(transform.forward, direction);

        //Compare the FOV to the dot product (Is within line of sight)
        if (dotProduct >= Mathf.Cos(fieldOfView))
        {
            //Raycast to check if sight was found
            if (Physics.Raycast(transform.position, direction, out RaycastHit hit, visionCollider.radius, lineOfSightLayers))
            {
                //Invoke gained sight event
                OnGainSight?.Invoke(Target);

                //Return succesful
                return true;
            }
        }

        //Return unsuccesful
        return false;
    }

    //Artificial delay for line of sight
    private IEnumerator CheckForLineOfSight(Transform givenTarget)
    {
        //The time to wait for
        WaitForSeconds waitTime = new WaitForSeconds(0.5f);

        //If not within line of sight, wait and try again
        while (!CheckLineOfSight(givenTarget))
        {
            yield return waitTime;
        }
    }
}
