using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace UI.Button_Actions
{
    public class SetToSliderValue : MonoBehaviour
    {
        [SerializeField]private TMP_Text text;
        [SerializeField] private Slider slider;
        [SerializeField] private List<AudioSource> soundToPlayWith;
        [SerializeField] private float playAudioCooldown=0.1f;

        private float timeStamp;
        public void UpdateToSliderValue(float value)
        {
            text.text = Mathf.RoundToInt(value * 100).ToString();
            
            if (timeStamp<Time.time)
            {
                timeStamp = Time.time + playAudioCooldown;
                Debug.Log("on cooldown");
            }
            else
            {
                return; 
            }
            
            foreach (var audioSource in soundToPlayWith)
            {
                if (audioSource.isPlaying) continue;
                
                audioSource.Play();
                return;
            }


            CreateNewAudioSource();
        }

        private void Start()
        {
            text.text = Mathf.RoundToInt(slider.value * 100).ToString();
        }

        private void CreateNewAudioSource()
        {
            var audioSource = Instantiate(new GameObject(), transform).AddComponent<AudioSource>();
            
            audioSource.clip = soundToPlayWith[0].clip;
            audioSource.outputAudioMixerGroup = soundToPlayWith[0].outputAudioMixerGroup;
            audioSource.playOnAwake = false;
            audioSource.volume = soundToPlayWith[0].volume;
            audioSource.spatialBlend = soundToPlayWith[0].spatialBlend;
            audioSource.reverbZoneMix = soundToPlayWith[0].reverbZoneMix;
            audioSource.dopplerLevel = soundToPlayWith[0].dopplerLevel;
            audioSource.spread = soundToPlayWith[0].spread;
            audioSource.rolloffMode = soundToPlayWith[0].rolloffMode;
            audioSource.minDistance = soundToPlayWith[0].minDistance;
            audioSource.maxDistance = soundToPlayWith[0].maxDistance;
            
            soundToPlayWith.Add(audioSource);
        }
    }
}
