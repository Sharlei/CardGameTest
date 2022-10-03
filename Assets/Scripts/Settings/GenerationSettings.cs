using UnityEngine;

namespace Settings
{
    [CreateAssetMenu(fileName = "GenerationSettings", menuName = "Settings/GenerationSettings")]
    public class GenerationSettings : ScriptableObject
    {
        public string ImageUri => string.Format(imageUri, imageWidth, imageHeight);

        [SerializeField] private string imageUri;
        
        public int maxCardsCount;
        public string defaultName;
        public string defaultDescription;
        public int minValue;
        public int maxValue;
        public int imageWidth;
        public int imageHeight;
    }
}
