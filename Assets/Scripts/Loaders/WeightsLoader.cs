using System;
using System.Collections.Generic;
using Loaders.Data;
using Map;
using ShipLogic;
using UnityEngine;
using Utils;
using Utils.Loader;

namespace Loaders
{
    public class WeightsLoader : ILoader
    {
        private struct WeightWrapper
        {
            public readonly MapObjectType MapObjectType;
            public readonly ShipType? ShipType;
            public readonly float Weight;

            public static WeightWrapper CreateWeightForShip(ShipType shipType, float weight)
            {
                return new WeightWrapper(MapObjectType.Ship, shipType, weight);
            }

            public static WeightWrapper CreateWeightForPlanet(float weight)
            {
                return new WeightWrapper(MapObjectType.Planet, null, weight);
            }
            
            private WeightWrapper(MapObjectType mapObjectType, ShipType? shipType, float weight)
            {
                MapObjectType = mapObjectType;
                ShipType = shipType;
                Weight = weight;
            }
        }

        public float MinWeight { get; private set; }
        public float MaxWeight { get; private set; }
        
        private readonly Dictionary<ShipType, List<WeightWrapper>> _weightsForShips = new();

        private const string PathFile = "Json/WeightsData";

        public float GetWeightForSelectedShip(ShipType selectedShip, ShipType otherShip)
        {
            if (!_weightsForShips.ContainsKey(selectedShip))
            {
                Debug.LogError($"Weight for ships is null for type: {selectedShip}");
                return -1;
            }
            
            foreach (var weightData in _weightsForShips[selectedShip])
            {
                if (weightData.MapObjectType != MapObjectType.Ship)
                {
                    continue;
                }

                if (weightData.ShipType == otherShip)
                {
                    return weightData.Weight;
                }
            }

            return -1;
        }
        
        public void Load()
        {
            var weightsData = StaticLoader.LoadJson<WeightsData>(PathFile);
            MinWeight = weightsData.MinWeight;
            MaxWeight = weightsData.MaxWeight;

            foreach (var weightShip in weightsData.WeightShips)
            {
                var shipType = weightShip.ShipType.ParseEnum<ShipType>();
                if (!_weightsForShips.ContainsKey(shipType))
                {
                    _weightsForShips[shipType] = new List<WeightWrapper>();
                }
                
                foreach (var weightData in weightShip.Weights)
                {
                    var mapObjectType = weightData.MapObjectType.ParseEnum<MapObjectType>();
                    switch (mapObjectType)
                    {
                        case MapObjectType.Empty:
                            Debug.LogError($"Not corrected type: {MapObjectType.Empty}");
                            break;
                        case MapObjectType.Ship:
                            var selectedShipType = weightData.ShipType.ParseEnum<ShipType>();
                            _weightsForShips[shipType].Add(WeightWrapper.CreateWeightForShip(selectedShipType, weightData.Weight));
                            break;
                        case MapObjectType.Planet:
                            _weightsForShips[shipType].Add(WeightWrapper.CreateWeightForPlanet(weightData.Weight));
                            break;
                        default:
                            throw new ArgumentOutOfRangeException();
                    }
                }
            }
        }
    }
}