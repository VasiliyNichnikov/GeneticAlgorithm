using System.Collections.Generic;
using Loaders;
using ShipLogic.Strategy.Attack;
using UnityEngine;

namespace ShipLogic
{
    public class WeightCalculator
    {
        private readonly IShipAttackLogic _shipAttackLogic;
        private static Dictionary<ShipType, float> _loadedWeights;

        public WeightCalculator(ShipType ship, IShipAttackLogic shipAttackLogic)
        {
            _shipAttackLogic = shipAttackLogic;

            if (_loadedWeights == null)
            {
                Main.Instance.LoaderManager.LoadAsync<WeightsLoader>(loader =>
                {
                    _loadedWeights = new Dictionary<ShipType, float>
                    {
                        { ShipType.Stealth, loader.GetWeightForSelectedShip(ship, ShipType.Stealth) },
                        { ShipType.Fighter, loader.GetWeightForSelectedShip(ship, ShipType.Fighter) },
                        { ShipType.Mining, loader.GetWeightForSelectedShip(ship, ShipType.Mining) },
                        { ShipType.AircraftCarrier, loader.GetWeightForSelectedShip(ship, ShipType.AircraftCarrier) }
                    };
                }, false);
            }
        }
        
        public float GetWeight()
        {
            var alliesWeight = GetWeightForShips(_shipAttackLogic.Allies);
            var enemyWeight = GetWeightForShips(_shipAttackLogic.Enemies);

            if (alliesWeight > enemyWeight)
            {
                return 0.0f;
            }

            return enemyWeight - alliesWeight;
        }

        private float GetWeightForShips(IEnumerable<ShipBase> ships)
        {
            var result = 0.0f;
            foreach (var enemy in ships)
            {
                if (!_loadedWeights.ContainsKey(enemy.Type))
                {
                    Debug.LogError($"Not found weight for ship: {enemy.Type}");
                    continue;
                }

                var weight = _loadedWeights[enemy.Type];
                result += weight;
            }

            return result;
        }
    }
}