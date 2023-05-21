using System;
using System.Linq;
using UnityEngine;

namespace ShipLogic
{
    /// <summary>
    /// Хранит все диапазоны в которых может работать корабль
    /// </summary>
    public class MainShipParameters : MonoBehaviour
    {
        [Serializable]
        private struct Parameter
        {
            public ParameterType Type => _type;
            public float MinValue => _minValue;
            public float MaxValue => _maxValue;

            [SerializeField] private ParameterType _type;
            [SerializeField] private float _minValue;
            [SerializeField] private float _maxValue;
        }
        
        public enum ParameterType
        {
            SpeedMovement,
            VisibilityRadius,
            RateOfFire,
            GunPower,
            Armor,
            Health
        }

        [SerializeField]
        private Parameter[] _parameters;

        public (float minValue, float maxValue) GetParameterByType(ParameterType type)
        {
            var foundParameter = _parameters.FirstOrDefault(p => p.Type == type);
            return (foundParameter.MinValue, foundParameter.MaxValue);
        }
    }
}