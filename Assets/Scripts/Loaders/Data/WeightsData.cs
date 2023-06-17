using System.Collections.Generic;
using JetBrains.Annotations;

namespace Loaders.Data
{
    public struct WeightsData
    {
        public struct WeightData
        {
            public string MapObjectType { get; set; }
            [CanBeNull] public string ShipType { get; set; }
            public float Weight { get; set; }
        }
        
        public struct WeightShip
        {
            public string ShipType { get; set; }
            public List<WeightData> Weights { get; set; }
        }
        
        public float MinWeight { get; set; }
        public float MaxWeight { get; set; }
        public List<WeightShip> WeightShips { get; set; }
    }
}