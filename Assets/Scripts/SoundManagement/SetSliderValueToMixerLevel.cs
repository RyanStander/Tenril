using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace SoundManagement
{
    public class SetSliderValueToMixerLevel : MonoBehaviour
    {
        [SerializeField] private Slider slider;
        [SerializeField] private TMP_Text text;
        [SerializeField] private MixerType mixerType;

        private void Start()
        {
            var value = PlayerPrefs.GetFloat(mixerType.ToString());

            var modifiedValue=Mathf.Pow(10, value/20);
            
            slider.value= modifiedValue;
            text.text= Mathf.RoundToInt(modifiedValue * 100).ToString();
        }
    }
}
