using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DestroyObjectAfterWait : MonoBehaviour
{
    [SerializeField] private float lifeTime;
    private float timeUntilDestroy;

    private void Start()
    {
        timeUntilDestroy = Time.time + lifeTime;
    }
    private void Update()
    {
        if (timeUntilDestroy< Time.time)
        {
            Destroy(gameObject);
        }
    }
}
