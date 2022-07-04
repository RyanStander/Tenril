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
            audioMixer.SetFloat(mixerName.ToString(), Mathf.Log10(value)*20);
        }
    }
}
