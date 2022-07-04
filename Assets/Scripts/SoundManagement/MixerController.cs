using UnityEngine;
using UnityEngine.Audio;

namespace SoundManagement
{
    public class MixerController : MonoBehaviour
    {
        [SerializeField] private AudioMixer audioMixer;
        [SerializeField] private MixerType mixerName;

        public void SetVolume(float value)
        {
            var volumeLevel = Mathf.Log10(value) * 20;

            audioMixer.SetFloat(mixerName.ToString(), volumeLevel);
            PlayerPrefs.SetFloat(mixerName.ToString(),volumeLevel);
        }
    }
}
