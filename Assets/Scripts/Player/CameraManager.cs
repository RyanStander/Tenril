using Cinemachine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraManager : MonoBehaviour
{
    [SerializeField] private GameObject explorationCamera;
    [SerializeField] private GameObject menuCamera;
    [SerializeField] private GameObject lockOnCamera;

    private GameObject npcCamera;

    private void Awake()
    {
        EventManager.currentManager.Subscribe(EventType.SwapToExplorationCamera, OnSwapToExplorationCamera);
        EventManager.currentManager.Subscribe(EventType.SwapToLockOnCamera, OnSwapToLockOnCamera);
        EventManager.currentManager.Subscribe(EventType.SwapToMenuCamera, OnSwapToMenuCamera);
        EventManager.currentManager.Subscribe(EventType.SwapToNPCCamera, OnSwapToNPCCamera);
    }

    #region onEvents

    private void OnSwapToExplorationCamera(EventData eventData)
    {
        if (eventData is SwapToExplorationCamera)
        {
            //Enable explorationCamera
            explorationCamera.SetActive(true);
            //disable all other cameras
            menuCamera.SetActive(false);
            lockOnCamera.SetActive(false);
            if (npcCamera != null)
            {
                npcCamera.SetActive(false);
                npcCamera = null;
            }
        }
        else
            throw new System.Exception("Error: EventData class with EventType.SwapToExplorationCamera was received but is not of class SwapToExplorationCamera.");
    }

    private void OnSwapToLockOnCamera(EventData eventData)
    {
        if (eventData is SwapToLockOnCamera)
        {
            //Enable lockOnCamera
            lockOnCamera.SetActive(true);
            //disable all other cameras
            menuCamera.SetActive(false);
            explorationCamera.SetActive(false);
            if (npcCamera != null)
            {
                npcCamera.SetActive(false);
                npcCamera = null;
            }
        }
        else
            throw new System.Exception("Error: EventData class with EventType.SwapToLockOnCamera was received but is not of class SwapToLockOnCamera.");
    }

    private void OnSwapToMenuCamera(EventData eventData)
    {
        if (eventData is SwapToMenuCamera)
        {
            //Enable lockOnCamera
            menuCamera.SetActive(true);
            //disable all other cameras
            lockOnCamera.SetActive(false);
            explorationCamera.SetActive(false);
            if (npcCamera != null)
            {
                npcCamera.SetActive(false);
                npcCamera = null;
            }
        }
        else
            throw new System.Exception("Error: EventData class with EventType.SwapToMenuCamera was received but is not of class SwapToMenuCamera.");
    }

    private void OnSwapToNPCCamera(EventData eventData)
    {
        if (eventData is SwapToNPCCamera swapToNPCCamera)
        {
            //set npcCamera
            npcCamera=swapToNPCCamera.npcCamera;

            //Enable lockOnCamera
            npcCamera.SetActive(true);
            //disable all other cameras
            menuCamera.SetActive(false);
            explorationCamera.SetActive(false);
            lockOnCamera.SetActive(false);
        }
        else
            throw new System.Exception("Error: EventData class with EventType.SwapToMenuCamera was received but is not of class SwapToMenuCamera.");
    }
    #endregion
}
