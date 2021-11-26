using Cinemachine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraManager : MonoBehaviour
{
    [SerializeField] private GameObject explorationCamera;
    [SerializeField] private GameObject menuCamera;
    [SerializeField] private GameObject lockOnCamera;

    private CinemachineInputProvider explorationCameraInputProvider;
    private GameObject npcCamera;

    private void Awake()
    {
        EventManager.currentManager.Subscribe(EventType.SwapToExplorationCamera, OnSwapToExplorationCamera);
        EventManager.currentManager.Subscribe(EventType.SwapToLockOnCamera, OnSwapToLockOnCamera);
        EventManager.currentManager.Subscribe(EventType.SwapToMenuCamera, OnSwapToMenuCamera);
        EventManager.currentManager.Subscribe(EventType.SwapToNPCCamera, OnSwapToNPCCamera);
    }

    private void Start()
    {
        SetupCameras();
    }

    private void SetupCameras()
    {
        GameObject player = GameObject.FindGameObjectWithTag("Player");

        //setup the exploration camera values
        if (explorationCamera != null)
        {
            CinemachineFreeLook explorationCameraFreeLookCam = explorationCamera.GetComponent<CinemachineFreeLook>();
            if (explorationCameraFreeLookCam != null)
            {
                if (explorationCameraFreeLookCam.Follow == null)
                    explorationCameraFreeLookCam.Follow = player.transform;

                if (explorationCameraFreeLookCam.LookAt == null)
                    explorationCameraFreeLookCam.LookAt = player.transform;

                explorationCameraInputProvider = explorationCamera.GetComponent<CinemachineInputProvider>();
            }
        }
        //setup the menu camera values
        if (menuCamera != null)
        {
            CinemachineVirtualCamera menuCameraVirtual = menuCamera.GetComponent<CinemachineVirtualCamera>();
            if (menuCameraVirtual != null)
            {
                if (menuCameraVirtual.Follow == null)
                    menuCameraVirtual.Follow = player.transform;

                if (menuCameraVirtual.LookAt == null)
                {
                    Transform spine = player.transform.Find("Root/Hips/Spine/Spine1");
                    if (spine != null)
                        menuCameraVirtual.LookAt = spine;
                }
            }
        }
    }

    #region onEvents

    private void OnSwapToExplorationCamera(EventData eventData)
    {
        if (eventData is SwapToExplorationCamera)
        {
            //Enable explorationCamera
            explorationCamera.SetActive(true);
            explorationCameraInputProvider.enabled = true;
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
            explorationCameraInputProvider.enabled = false;
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
            explorationCameraInputProvider.enabled = false;
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
            explorationCameraInputProvider.enabled = false;
            lockOnCamera.SetActive(false);
        }
        else
            throw new System.Exception("Error: EventData class with EventType.SwapToMenuCamera was received but is not of class SwapToMenuCamera.");
    }
    #endregion
}
