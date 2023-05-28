using System.Globalization;
using UnityEngine;
using UnityEngine.UI;
using Utils;

namespace UI.Dialog.GameController
{
    public class SpeedGame : MonoBehaviour
    {
        [SerializeField] private Slider _speedValue;
        [SerializeField] private Text _speedText;

        private float _minSpeed;
        private float _maxSpeed;
        
        public void Init(int minSpeed, int maxSpeed)
        {
            _minSpeed = minSpeed;
            _maxSpeed = maxSpeed;
            
            _speedText.text = _minSpeed.ToString(CultureInfo.InvariantCulture);
            Time.timeScale = _minSpeed;
            _speedValue.value = ConvertSpeedFromZeroToOne(minSpeed);
            _speedValue.onValueChanged.AddListener(OnChangeSpeed);
        }

        private void OnChangeSpeed(float value)
        {
            var correctedValueFloat = Converter.ConvertFromOneRangeToAnother(0, 1, _minSpeed, _maxSpeed, value);
            var correctedValueInt = Mathf.RoundToInt(correctedValueFloat);
            _speedText.text = correctedValueInt.ToString();
            Time.timeScale = correctedValueInt;
        }

        private float ConvertSpeedFromZeroToOne(float currentValue)
        {
            return Converter.ConvertFromOneRangeToAnother(_minSpeed, _maxSpeed, 0, 1, currentValue);
        }
    }
}