using UnityEngine;
using UnityEngine.UI;
using Utils;

namespace UI.Dialog.InfoAboutMiningPlanet
{
    public class CapturingPoint : MonoBehaviour
    {
        [SerializeField] private Text _playerName;
        [SerializeField] private Text _timeLeft;
        [SerializeField] private Slider _capturePercentage;

        private void Start()
        {
            ResetName();
            ResetSlider();
        }

        public void SetPlayerName(string playerName)
        {
            _playerName.text = playerName;
        }

        public void SetSlider(float value, float maxValue)
        {
            if (value < 0 || value > 1)
            {
                Debug.LogWarning($"Not corrected value for slider: {value}");
                return;
            }

            var timeLeftValue = Mathf.RoundToInt(Converter.ConvertFromOneRangeToAnother(0, 1, 0, maxValue, value));
            _capturePercentage.value = value;
            _timeLeft.text = $"{timeLeftValue}/{maxValue}";
        }

        public void ResetName()
        {
            _playerName.text = "N/A";
        }
        
        public void ResetSlider()
        {
            _capturePercentage.value = 0.0f;
            _timeLeft.text = string.Empty;
        }
    }
}