using System;
using UnityEngine;

namespace Player.CameraScripts
{
    /// <summary>
    /// Sets the ui camera's fov to the main camera's fov
    /// </summary>
    public class CameraFovSetter : MonoBehaviour
    {
        [SerializeField] private Camera mainCamera,uiCamera;

        private float currentFov;
        private void LateUpdate()
        {
            //if the current fov does not match the set fov
            if (Math.Abs(mainCamera.fieldOfView - currentFov) < 0.01f)
                return;

            currentFov = mainCamera.fieldOfView;
            uiCamera.fieldOfView = currentFov;
        }
    }
}
