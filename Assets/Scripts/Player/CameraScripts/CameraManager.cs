using Cinemachine;
using UnityEngine;

namespace Player.CameraScripts
{
    public class CameraManager : MonoBehaviour
    {
        [SerializeField] private GameObject explorationCamera, menuCamera, lockOnCamera, aimCamera;
        [SerializeField] private CameraLockOn cameraLockOnComponent;

        private CinemachineInputProvider explorationCameraInputProvider;
        private GameObject npcCamera;

        private CinemachineFreeLook explorationCameraFreeLookCam;

        [Header("Camera settings")] [SerializeField]
        private float ySensitivtyKeyboard = 0.0015f;

        [SerializeField] private float xSensitivtyKeyboard = 0.15f;
        [SerializeField] private float ySensitivtyController = 2;
        [SerializeField] private float xSensitivtyController = 300;

        private void Awake()
        {
            EventManager.currentManager.Subscribe(EventType.SwapToExplorationCamera, OnSwapToExplorationCamera);
            EventManager.currentManager.Subscribe(EventType.SwapToLockOnCamera, OnSwapToLockOnCamera);
            EventManager.currentManager.Subscribe(EventType.SwapToAimCamera, OnSwapToAimCamera);
            EventManager.currentManager.Subscribe(EventType.SwapToMenuCamera, OnSwapToMenuCamera);
            EventManager.currentManager.Subscribe(EventType.SwapToNPCCamera, OnSwapToNPCCamera);
            EventManager.currentManager.Subscribe(EventType.PlayerChangedInputDevice, OnPlayerChangedInputDevice);
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
                explorationCameraFreeLookCam = explorationCamera.GetComponent<CinemachineFreeLook>();
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

            //setup the aim camera values
            if (aimCamera != null)
            {
                if (aimCamera.TryGetComponent(out CinemachineVirtualCamera cinemachineVirtualCamera))
                    cinemachineVirtualCamera.Follow = GameObject.FindGameObjectWithTag("CameraFollowTarget").transform;

                if (aimCamera.TryGetComponent(out CrosshairAimAdjustment crosshair))
                    crosshair.aimTargetReticle =
                        GameObject.FindGameObjectWithTag("Crosshair").GetComponent<RectTransform>();
            }
        }

        private void DisableAllCameras()
        {
            explorationCamera.SetActive(false);
            aimCamera.SetActive(false);
            lockOnCamera.SetActive(false);
            menuCamera.SetActive(false);
            if (npcCamera != null)
            {
                npcCamera.SetActive(false);
                npcCamera = null;
            }
        }

        #region onEvents

        private void OnSwapToExplorationCamera(EventData eventData)
        {
            if (eventData is SwapToExplorationCamera)
            {
                DisableAllCameras();

                //Enable explorationCamera
                explorationCamera.SetActive(true);
                explorationCameraInputProvider.enabled = true;
            }
            else
                throw new System.Exception(
                    "Error: EventData class with EventType.SwapToExplorationCamera was received but is not of class SwapToExplorationCamera.");
        }

        private void OnSwapToAimCamera(EventData eventData)
        {
            if (eventData is SwapToAimCamera)
            {
                DisableAllCameras();

                //Enable explorationCamera
                aimCamera.SetActive(true);
            }
            else
                throw new System.Exception(
                    "Error: EventData class with EventType.SwapToExplorationCamera was received but is not of class SwapToExplorationCamera.");
        }

        private void OnSwapToLockOnCamera(EventData eventData)
        {
            if (eventData is SwapToLockOnCamera)
            {
                if (!cameraLockOnComponent.HasLockOnTarget()) return;

                DisableAllCameras();

                //Enable lockOnCamera
                lockOnCamera.SetActive(true);
                DisableAllCameras();

                //Enable lockOnCamera
                lockOnCamera.SetActive(true);
            }
            else
                throw new System.Exception(
                    "Error: EventData class with EventType.SwapToLockOnCamera was received but is not of class SwapToLockOnCamera.");
        }

        private void OnSwapToMenuCamera(EventData eventData)
        {
            if (eventData is SwapToMenuCamera)
            {
                DisableAllCameras();

                //Enable lockOnCamera
                menuCamera.SetActive(true);
            }
            else
                throw new System.Exception(
                    "Error: EventData class with EventType.SwapToMenuCamera was received but is not of class SwapToMenuCamera.");
        }

        private void OnSwapToNPCCamera(EventData eventData)
        {
            if (eventData is SwapToNpcCamera swapToNPCCamera)
            {
                DisableAllCameras();

                //set npcCamera
                npcCamera = swapToNPCCamera.npcCamera;

                //Enable lockOnCamera
                npcCamera.SetActive(true);
            }
            else
                throw new System.Exception(
                    "Error: EventData class with EventType.SwapToMenuCamera was received but is not of class SwapToMenuCamera.");
        }

        private void OnPlayerChangedInputDevice(EventData eventData)
        {
            if (eventData is PlayerChangedInputDevice playerChangedInputDevice)
            {
                switch (playerChangedInputDevice.inputDevice)
                {
                    case InputDeviceType.KeyboardMouse:
                        ChangeToKeyboardSettings();
                        break;
                    case InputDeviceType.GeneralGamepad:
                        ChangeToControllerSettings();
                        break;
                    case InputDeviceType.PlayStation:
                        ChangeToControllerSettings();
                        break;
                    case InputDeviceType.Xbox:
                        ChangeToControllerSettings();
                        break;
                }
            }
            else
                throw new System.Exception(
                    "Error: EventData class with EventType.PlayerChangedInputDevice was received but is not of class PlayerChangedInputDevice.");
        }

        #endregion

        private void ChangeToKeyboardSettings()
        {
            //Set the camera to input value gain mode
            explorationCameraFreeLookCam.m_YAxis.m_SpeedMode = AxisState.SpeedMode.InputValueGain;
            explorationCameraFreeLookCam.m_XAxis.m_SpeedMode = AxisState.SpeedMode.InputValueGain;

            //Set the camera speed/sensitivity
            explorationCameraFreeLookCam.m_YAxis.m_MaxSpeed = ySensitivtyKeyboard;
            explorationCameraFreeLookCam.m_XAxis.m_MaxSpeed = xSensitivtyKeyboard;
        }

        private void ChangeToControllerSettings()
        {
            //Set the camera to input value gain mode
            explorationCameraFreeLookCam.m_YAxis.m_SpeedMode = AxisState.SpeedMode.MaxSpeed;
            explorationCameraFreeLookCam.m_XAxis.m_SpeedMode = AxisState.SpeedMode.MaxSpeed;

            //Set the camera speed/sensitivity
            explorationCameraFreeLookCam.m_YAxis.m_MaxSpeed = ySensitivtyController;
            explorationCameraFreeLookCam.m_XAxis.m_MaxSpeed = xSensitivtyController;
        }
    }
}