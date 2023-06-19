using System.Collections.Generic;
using Loaders.Data;
using UnityEngine;
using Utils;
using Utils.Loader;

namespace Loaders
{
    public class MiningPlanetLoader : ILoader
    {
        private enum ParameterType
        {
            MinimumTimeMining,
            MaximumTimeMining,
            AccelerationPercentageForEachShip,
            AmountOfGoldPerExtraction
        }

        private readonly Dictionary<ParameterType, float> _parameters = new();
        private const string PathFile = "Json/MiningData";

        public float GetMinimumTimeMining()
        {
            return GetValueFromParameters(ParameterType.MinimumTimeMining);
        }

        public float GetMaximumTimeMining()
        {
            return GetValueFromParameters(ParameterType.MaximumTimeMining);
        }

        public float AccelerationPercentageForEachShip()
        {
            return GetValueFromParameters(ParameterType.AccelerationPercentageForEachShip);
        }

        public float GetAmountOfGoldPerExtraction()
        {
            return GetValueFromParameters(ParameterType.AmountOfGoldPerExtraction);
        }
        
        
        public void Load()
        {
            var miningData = StaticLoader.LoadJson<MiningData>(PathFile);

            foreach (var parameterData in miningData.Parameters)
            {
                var parameterType = parameterData.Name.ParseEnum<ParameterType>();
                if (_parameters.ContainsKey(parameterType))
                {
                    Debug.LogError("Parameter contains in list");
                    continue;
                }

                _parameters.Add(parameterType, parameterData.Value);
            }
        }

        private float GetValueFromParameters(ParameterType parameter)
        {
            if (!_parameters.ContainsKey(parameter))
            {
                Debug.LogWarning($"Not found parameter ({parameter})");
                return 0.0f;
            }
            
            return _parameters[parameter];
        }
    }
}