using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using Utils;

namespace ShipLogic.Editor
{
    [CreateAssetMenu(fileName = "CustomCharacteristics", menuName = "Ship/CustomCharacteristics", order = 1)]
    public class ShipCharacteristics : ScriptableObject
    {
        [SerializeField] private float _minWeight;
        [SerializeField] private float _maxWeight;

        [SerializeField] private float _speedMovement;
        [SerializeField] private float _rateOfFire;
        // todo так как правила изменены, лучше сделать вариант с процентом поглощения
        [SerializeField] private float _visibilityRadius; 
        [SerializeField] private float _gunPower;
        [SerializeField] private float _armor;

        public ShipData ConvertToShipData()
        {
            var mainParameters = Main.Instance.ShipParameters;

            var speedMovement = ConvertSelectedParameter(
                mainParameters.GetParameterByType(MainShipParameters.ParameterType.SpeedMovement), _speedMovement);

            var rateOfFire = ConvertSelectedParameter(
                mainParameters.GetParameterByType(MainShipParameters.ParameterType.RateOfFire), _rateOfFire);

            var visibilityRadius = ConvertSelectedParameter(
                mainParameters.GetParameterByType(MainShipParameters.ParameterType.VisibilityRadius),
                _visibilityRadius);

            var gunPower = ConvertSelectedParameter(
                mainParameters.GetParameterByType(MainShipParameters.ParameterType.GunPower), _gunPower);

            var armor = ConvertSelectedParameter(
                mainParameters.GetParameterByType(MainShipParameters.ParameterType.Armor), _armor);

            return new ShipData(speedMovement, rateOfFire, visibilityRadius, gunPower, armor, GetSkinForShip());
        }

        private float ConvertSelectedParameter((float minNew, float maxNew) newParameters, float currentValue)
        {
            if (currentValue < _minWeight || currentValue > _maxWeight)
            {
                Debug.LogError($"Not corrected current value: {currentValue}, range: [{_minWeight}, {_maxWeight}]");
                return -1f;
            }

            return Converter.ConvertFromOneRangeToAnother(_minWeight, _maxWeight, newParameters.minNew,
                newParameters.maxNew, currentValue);
        }

        // todo потом нужно будет вынести в какое-то место)
        private ShipSkinData.SkinType GetSkinForShip()
        {
            var weights = new Dictionary<ShipSkinData.SkinType, float>()
            {
                { ShipSkinData.SkinType.Stealth, 0 },
                { ShipSkinData.SkinType.Fighter, 0 },
                { ShipSkinData.SkinType.HeavyShip, 0 }
            };

            // Настройка параметра скорости
            switch (_speedMovement)
            {
                case >= 0.4f:
                    weights[ShipSkinData.SkinType.Stealth] += 1;
                    break;
                case < 0.4f and >= 0.2f:
                    weights[ShipSkinData.SkinType.Fighter] += 1;
                    break;
                default:
                    weights[ShipSkinData.SkinType.HeavyShip] += 1;
                    break;
            }

            // Настройка параметра видимости
            switch (_visibilityRadius)
            {
                case >= 0.4f:
                    weights[ShipSkinData.SkinType.Stealth] += 1;
                    break;
                case < 0.4f and >= 0.2f:
                    weights[ShipSkinData.SkinType.Fighter] += 1;
                    break;
                default:
                    weights[ShipSkinData.SkinType.HeavyShip] += 1;
                    break;
            }
            
            // Настройка перезарядки снаряда
            switch (_rateOfFire)
            {
                case >= 0.4f:
                    weights[ShipSkinData.SkinType.HeavyShip] += 1;
                    break;
                case < 0.4f and >= 0.2f:
                    weights[ShipSkinData.SkinType.Fighter] += 1;
                    break;
                default:
                    weights[ShipSkinData.SkinType.Stealth] += 1;
                    break;
            }

            // Настройка мощности снаряда
            switch (_gunPower)
            {
                case >= 0.4f:
                    weights[ShipSkinData.SkinType.HeavyShip] += 1;
                    break;
                case < 0.4f and >= 0.2f:
                    weights[ShipSkinData.SkinType.Fighter] += 1;
                    break;
                default:
                    weights[ShipSkinData.SkinType.Stealth] += 1;
                    break;
            }

            // Настройка параметров брони
            switch (_armor)
            {
                case >= 0.4f:
                    weights[ShipSkinData.SkinType.HeavyShip] += 1;
                    break;
                case < 0.4f and >= 0.2f:
                    weights[ShipSkinData.SkinType.Fighter] += 1;
                    break;
                default:
                    weights[ShipSkinData.SkinType.Stealth] += 1;
                    break;
            }

            var maxWeight = weights.Select(kvp => kvp.Value).Max();
            var foundSkin = weights.FirstOrDefault(weight => Math.Abs(weight.Value - maxWeight) < 0.01f).Key;
            return foundSkin;
        }
        
    }
}