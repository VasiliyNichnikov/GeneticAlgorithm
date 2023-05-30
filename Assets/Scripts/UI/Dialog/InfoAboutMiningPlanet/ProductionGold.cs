using UnityEngine;
using UnityEngine.UI;
using Utils;

namespace UI.Dialog.InfoAboutMiningPlanet
{
    public class ProductionGold : MonoBehaviour
    {
        [SerializeField] private Text _playerName;
        [SerializeField] private Text _timeLeftText;
        [SerializeField] private Slider _capturePercentage;
        [SerializeField] private Image _fillImage;

        public void Init(Color fillColor, string playerName)
        {
            _fillImage.color = fillColor;
            _playerName.text = playerName;
        }
        
        public void SetSlider(float value, float minValue, float maxValue)
        {
            if (value < 0 || value > 1)
            {
                Debug.LogWarning($"Not corrected value for slider: {value}");
                return;
            }

            var timeLeftValue = Mathf.RoundToInt(Converter.ConvertFromOneRangeToAnother(0, 1, 0, maxValue, value));
            _capturePercentage.value = value;
            
            _timeLeftText.text = $"{timeLeftValue}/{maxValue}";
        }

        public void ResetSlider()
        {
            _capturePercentage.value = 0.0f;
            _timeLeftText.text = string.Empty;
        }

        public void ResetName()
        {
            _playerName.text = "N/A";
        }
    }
}