using System;
using TMPro;
using UnityEngine;

namespace UI.Video
{
    public class GraphicsSetting : MonoBehaviour
    {
        [SerializeField]private TMP_Dropdown dropdown;
        private void Start()
        {
            dropdown.value = QualitySettings.GetQualityLevel();
            dropdown.RefreshShownValue();
        }

        public void SetQuality(int index)
        {
            QualitySettings.SetQualityLevel(index);
        }
    }
}
