using System;
using System.Collections.Generic;
using Loaders.Data;
using Map;
using Planets;
using Players;
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
            public readonly PlanetType? PlanetType;
            public readonly float Weight;

            public static WeightWrapper CreateWeightForShip(ShipType shipType, float weight)
            {
                return new WeightWrapper(MapObjectType.Ship, shipType, null, weight);
            }

            public static WeightWrapper CreateWeightForPlanet(PlanetType planetType, float weight)
            {
                return new WeightWrapper(MapObjectType.Planet, null, planetType, weight);
            }

            private WeightWrapper(MapObjectType mapObjectType, ShipType? shipType, PlanetType? planetType, float weight)
            {
                MapObjectType = mapObjectType;
                ShipType = shipType;
                PlanetType = planetType;
                Weight = weight;
            }
        }

        public float MinWeight { get; private set; }
        public float MaxWeight { get; private set; }

        private readonly List<WeightWrapper> _weights = new();

        private const string PathFile = "Json/WeightsData";

        public float GetWeightForSelectedShip(ShipType ship)
        {
            foreach (var weightData in _weights)
            {
                if (weightData.MapObjectType != MapObjectType.Ship)
                {
                    continue;
                }

                if (weightData.ShipType == ship)
                {
                    return weightData.Weight;
                }
            }

            return -1;
        }

        public float GetWeightForSelectedPlanet(PlanetType planet)
        {
            foreach (var weightData in _weights)
            {
                if (weightData.MapObjectType != MapObjectType.Planet)
                {
                    continue;
                }

                if (weightData.PlanetType == planet)
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

            foreach (var weightData in weightsData.Weights)
            {
                var mapObjectType = weightData.MapObjectType.ParseEnum<MapObjectType>();
                switch (mapObjectType)
                {
                    case MapObjectType.Empty:
                        Debug.LogError($"Not corrected type: {MapObjectType.Empty}");
                        break;
                    case MapObjectType.Ship:
                        var selectedShip = weightData.ShipType.ParseEnum<ShipType>();
                        _weights.Add(WeightWrapper.CreateWeightForShip(selectedShip, weightData.Weight));
                        break;
                    case MapObjectType.Planet:
                        var selectedPlanet = weightData.PlanetType.ParseEnum<PlanetType>();
                        _weights.Add(WeightWrapper.CreateWeightForPlanet(selectedPlanet, weightData.Weight));
                        break;
                    default:
                        throw new ArgumentOutOfRangeException();
                }
            }
        }
    }
}