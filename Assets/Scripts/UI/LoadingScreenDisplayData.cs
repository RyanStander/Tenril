using UnityEngine;

namespace UI
{
    [CreateAssetMenu(menuName = "ScriptableObjects/UI/LoadingScreenDisplayData")]
    public class LoadingScreenDisplayData : ScriptableObject
    {
        [Tooltip("The image associated with the info")]
        public Sprite displayImage;
        [Tooltip("The text displayed during the loading screen")]
        [TextArea]public string displayText;
        [Tooltip("How long the info is to be displayed")]
        public float displayTime = 6f;
    }
}
