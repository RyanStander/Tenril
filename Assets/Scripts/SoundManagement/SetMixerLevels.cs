using System;
using UnityEngine;
using UnityEngine.Audio;

namespace SoundManagement
{
    public class SetMixerLevels : MonoBehaviour
    {
        [SerializeField] private AudioMixer audioMixer;
        private void Start()
        {
            if (PlayerPrefs.HasKey(MixerType.MasterVolume.ToString()))
            {
                SetAudioMixerLevels();
            }
            else
            {
                CreateVolumePrefs();
            }
        }

        private void SetAudioMixerLevels()
        {
            var master = MixerType.MasterVolume.ToString();
            audioMixer.SetFloat("MasterVolume", PlayerPrefs.GetFloat(master));

            var ambience = MixerType.AmbienceVolume.ToString();
            audioMixer.SetFloat(ambience, PlayerPrefs.GetFloat(ambience));

            var dialogue = MixerType.DialogueVolume.ToString();
            audioMixer.SetFloat(dialogue, PlayerPrefs.GetFloat(dialogue));
            
            var menu = MixerType.MenuVolume.ToString();
            audioMixer.SetFloat(menu, PlayerPrefs.GetFloat(menu));
            
            var music = MixerType.MusicVolume.ToString();
            audioMixer.SetFloat(music, PlayerPrefs.GetFloat(music));
            
            var sfx = MixerType.SfxVolume.ToString();
            audioMixer.SetFloat(sfx, PlayerPrefs.GetFloat(sfx));
        }

        private void CreateVolumePrefs()
        {
            var master = MixerType.MasterVolume.ToString();
            PlayerPrefs.SetFloat(master, 0);
            
            var ambience = MixerType.AmbienceVolume.ToString();
            PlayerPrefs.SetFloat(ambience, 0);
            
            var dialogue = MixerType.DialogueVolume.ToString();
            PlayerPrefs.SetFloat(dialogue, 0);
            
            var menu = MixerType.MenuVolume.ToString();
            PlayerPrefs.SetFloat(menu, 0);
            
            var music = MixerType.MusicVolume.ToString();
            PlayerPrefs.SetFloat(music, 0);
            
            var sfx = MixerType.SfxVolume.ToString();
            PlayerPrefs.SetFloat(sfx, 0);
        }
    }
}
