using UnityEngine;
using UnityEngine.UI;
using Utils;

namespace UI.Dialog.InfoAboutPlayerPlanet
{
    public class CreatingShipTime : MonoBehaviour
    {
        [SerializeField] private Text _shipName;
        [SerializeField] private Text _timeLeft;
        [SerializeField] private Slider _slider;
        [SerializeField] private Image _fillBackground;

        private void Start()
        {
            ResetAll();
        }

        public void ResetAll()
        {
            ResetShipName();
            ResetSlider();
        }

        public void SetShipName(string shipName)
        {
            _shipName.text = shipName;
        }

        public void SetSlider(float currentValue, float minValue, float maxValue)
        {
            var valueClamped = Converter.ConvertFromOneRangeToAnother(minValue, maxValue, _slider.minValue, _slider.maxValue, currentValue);
            _slider.value = valueClamped;
            
            _timeLeft.text = $"{Mathf.RoundToInt(currentValue)}/{maxValue}";
        }
        
        public void SetColorSlider(Color color)
        {
            _fillBackground.color = color;
        }

        private void ResetShipName()
        {
            _shipName.text = "N/A";
        }
        
        private  void ResetSlider()
        {
            _slider.value = _slider.maxValue;
            _timeLeft.text = string.Empty;
        }
    }
}