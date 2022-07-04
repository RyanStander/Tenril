using TMPro;
using UnityEngine;

namespace UI.Video
{
    public class FullscreenSetting : MonoBehaviour
    {
        [SerializeField]private TMP_Dropdown dropdown;
        private void Start()
        {
            dropdown.value = (int)Screen.fullScreenMode;
            dropdown.RefreshShownValue();
        }
        
        public void SetFullscreen(int index)
        {
            Screen.fullScreenMode = (FullScreenMode)index;
        }
    }
}
