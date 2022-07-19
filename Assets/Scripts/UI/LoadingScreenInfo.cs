using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using Random = UnityEngine.Random;

namespace UI
{
    public class LoadingScreenInfo : MonoBehaviour
    {
        [SerializeField] private List<LoadingScreenDisplayData> loadingScreenDisplayDatas;
        [SerializeField] private Image loadingScreenImage;
        [SerializeField] private TMP_Text loadingScreenInfoText;

        [Tooltip("The duration that it spends to swap to a new tip")] [SerializeField]
        private float swapTime = 0.5f;


        private LoadingScreenDisplayData currentLoadingScreenDisplayData;
        private float timeStamp;
        private bool startedSwapping;
        private bool endingSwapping;

        private void Start()
        {
            var index = Random.Range(0, loadingScreenDisplayDatas.Count);
            
            currentLoadingScreenDisplayData = loadingScreenDisplayDatas[index];
            
            loadingScreenImage.sprite = currentLoadingScreenDisplayData.displayImage;
            loadingScreenInfoText.text = currentLoadingScreenDisplayData.displayText;
            timeStamp = Time.time + currentLoadingScreenDisplayData.displayTime;
            
            loadingScreenDisplayDatas.Remove(currentLoadingScreenDisplayData);
        }

        private void FixedUpdate()
        {
            //check if it is time to start changing tips
            if (timeStamp - swapTime / 2 < Time.time && !startedSwapping)
            {
                endingSwapping = false;
                startedSwapping = true;
                StartCoroutine(FadeOutLoadingScreens());
            }

            if (timeStamp < Time.time && !startedSwapping)
            {
                if (loadingScreenDisplayDatas.Count<1)
                {
                    return;
                }
                
                currentLoadingScreenDisplayData =
                    loadingScreenDisplayDatas[Random.Range(0, loadingScreenDisplayDatas.Count)];
                loadingScreenImage.sprite = currentLoadingScreenDisplayData.displayImage;
                loadingScreenInfoText.text = currentLoadingScreenDisplayData.displayText;
                timeStamp = Time.time + currentLoadingScreenDisplayData.displayTime;

                startedSwapping = false;
                endingSwapping = true;

                StartCoroutine(FadeInLoadingScreens());
            }
        }

        private IEnumerator FadeOutLoadingScreens()
        {
            while (startedSwapping)
            {
                var alphaValue = 225 * ((timeStamp - Time.time) / (swapTime / 2));
                //fade away
                //image fade
                var imgColor = loadingScreenImage.color;
                imgColor.a = alphaValue;
                loadingScreenImage.color = imgColor;
                //text fade
                var textColor = loadingScreenInfoText.color;
                textColor.a = alphaValue;
                loadingScreenInfoText.color = textColor;

                yield return null;
            }
        }

        private IEnumerator FadeInLoadingScreens()
        {
            while (endingSwapping)
            {
                var alphaValue =
                    225 * (1 - ((timeStamp - Time.time - (currentLoadingScreenDisplayData.displayTime - swapTime / 2)) /
                                (swapTime / 2)));
                //fade in
                //image fade
                var imgColor = loadingScreenImage.color;
                imgColor.a = alphaValue;
                loadingScreenImage.color = imgColor;
                //text fade
                var textColor = loadingScreenInfoText.color;
                textColor.a = alphaValue;
                loadingScreenInfoText.color = textColor;

                yield return null;
            }
        }
    }
}