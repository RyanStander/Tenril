using System;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace UI.Video
{
    public class ResolutionSetting : MonoBehaviour
    {
        private Resolution[] resolutions;
        [SerializeField]private TMP_Dropdown dropdown;

        private void Start()
        {
            resolutions = Screen.resolutions;
            
            dropdown.ClearOptions();
            
            var options = new List<string>();

            var currentResolutionIndex = 0;
            for (var index = 0; index < resolutions.Length; index++)
            {
                var resolution = resolutions[index];
                var option = resolution.width + "x" + resolution.height;
                options.Add(option);

                if (resolution.width == Screen.currentResolution.width &&
                    resolution.height == Screen.currentResolution.height)
                {
                    currentResolutionIndex = index;
                }
            }

            dropdown.AddOptions(options);
            dropdown.value = currentResolutionIndex;
            dropdown.RefreshShownValue();
        }

        public void SetResolution(int index)
        {
            var resolution = resolutions[index];
            Screen.SetResolution(resolution.width,resolution.height,Screen.fullScreen);
        }
    }
}