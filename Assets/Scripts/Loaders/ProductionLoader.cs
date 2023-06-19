using System.Collections.Generic;
using System.Linq;
using Loaders.Data;
using ShipLogic;
using UnityEngine;
using Utils;
using Utils.Loader;

namespace Loaders
{
    public class ProductionLoader : ILoader
    {
        private struct ShipProductionWrapper
        {
            public readonly ShipType Ship;
            public readonly float Price;
            public readonly float Time;

            public ShipProductionWrapper(ShipType ship, float price, float time)
            {
                Ship = ship;
                Price = price;
                Time = time;
            }
        }
        
        public float StartingGold { get; set; }
        
        private readonly List<ShipProductionWrapper> _shipProductionWrappers = new();
        private const string PathFile = "Json/ProductionData";

        public float GetPriceForShip(ShipType ship)
        {
            var productionPrice = _shipProductionWrappers.FirstOrDefault(data => data.Ship == ship);
            return productionPrice.Price;
        }
        
        public float GetTimeForShip(ShipType ship)
        {
            var productionPrice = _shipProductionWrappers.FirstOrDefault(data => data.Ship == ship);
            return productionPrice.Time;
        }
        
        public void Load()
        {
            var productionData = StaticLoader.LoadJson<ProductionData>(PathFile);
            
            StartingGold = productionData.StartingGold;
            foreach (var shipProduction in productionData.ShipProductions)
            {
                var shipType = shipProduction.ShipType.ParseEnum<ShipType>();
                if (shipProduction.Price < 0)
                {
                    Debug.LogError("The price cannot be less than 0");
                    continue;
                }

                if (shipProduction.Time < 0)
                {
                    Debug.LogError("The time cannot be less than 0");
                    continue;
                }

                var wrapper = new ShipProductionWrapper(shipType, shipProduction.Price, shipProduction.Time);
                _shipProductionWrappers.Add(wrapper);
            }
        }
    }
}